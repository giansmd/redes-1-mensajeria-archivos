# SR Chat — Mensajería y Transferencia de Archivos sobre Puerto Serie

Proyecto de Redes y Comunicaciones. Aplicación de chat con transferencia de archivos sobre comunicación serial RS-232 usando tramas de 1024 bytes.

## Arquitectura

- **Lenguaje:** C# (.NET 8, Windows Forms)
- **Proyectos:** `winProyComunicacionCOMA` (puerto COM10) y `winProyComunicacionCOMB` (puerto COM11)
- **Código compartido:** `Shared/classComunicacion.cs` (lógica de comunicación), `Shared/Form1.cs` (interfaz), `Shared/Form1.Designer.cs` (controles WinForms)

---

## Protocolo de Comunicación

Tramas de **1024 bytes** sobre puerto serie RS-232 (8 bits datos, 2 bits parada, paridad impar).

| Tipo | Cabecera (5 bytes) | Datos (1019 bytes) | Descripción |
|------|---------------------|---------------------|-------------|
| `M` | `"M" + longitud(4)` | mensaje UTF-8 + relleno `@` | Mensaje de texto |
| `A` | `"A" + "1019"` o `"A" + tamaño + "HHH"` | datos del archivo + relleno `@` | Chunk de archivo |
| `F` | `"F" + longitud(4)` | `"usuario|nombre|tamaño"` + relleno `@` | Metadatos de archivo |
| `I` | — | — | Idle / ignorar |

**Ejemplo:** Para enviar "Hola" (4 bytes): cabecera `"M0004"` + `"Hola"` + 1015 bytes de relleno `@`.

---

## Cambios Realizados (relación para el profesor)

### 1. Semáforos para Control de Concurrencia (`classComunicacion.cs:42-43`)

Se agregaron dos semáforos usando la clase `SemaphoreSlim` de .NET:

```csharp
private readonly SemaphoreSlim _semaforoPuerto = new SemaphoreSlim(1, 1);
private readonly SemaphoreSlim _semaforoEnvios = new SemaphoreSlim(5, 5);
```

| Variable | Conteo | Propósito |
|----------|--------|-----------|
| `_semaforoPuerto` | 1 (binario) | Exclusión mutua para escritura al `sPuerto` (SerialPort). Solo un hilo puede escribir al puerto serie a la vez. |
| `_semaforoEnvios` | 5 | Limita a máximo 5 hilos de envío de archivos concurrentes. |

**Métodos protegidos por `_semaforoPuerto`:**
- `EnviandoMensaje()` (línea 400) — envío de mensajes
- `LeyendoTransmitiendoArchivo()` (línea 282) — envío de archivos (modo heredado)
- `EnviarArchivoIndividual()` (línea 173) — envío de archivos (nuevo modo múltiple)

Esto garantiza que los mensajes y el envío de archivos no colisionen: si se está transmitiendo un chunk de archivo, el mensaje espera su turno, y viceversa. Ambos son independientes y uno no cancela al otro.

### 2. Envío de Archivos por Lotes (hasta 5 simultáneos) (`classComunicacion.cs:157-280`)

Nuevo método público `EnviarArchivos(string[] rutasArchivos)`:
- Recibe un arreglo con las rutas completas de los archivos
- Por cada archivo lanza un `Thread` que ejecuta `EnviarArchivoIndividual(rutaCompleta, nombreArchivo)`
- Cada hilo adquiere `_semaforoEnvios` (máximo 5 hilos simultáneos)
- Dentro del hilo, cada escritura al puerto serie adquiere `_semaforoPuerto`

**Flujo del hilo de envío individual:**
1. Abre el archivo con `FileStream` + `BinaryReader`
2. Envía trama `"F"` con metadatos (`"nombre|tamaño"`) — ver Fase 3
3. Lee chunks de 1019 bytes y envía tramas `"A"` por el puerto serie
4. Reporta progreso mediante el evento `ProgresoEnvio`
5. Al finalizar dispara `ArchivoEnvioCompletado`

Se conserva el método original `LeyendoTransmitiendoArchivo()` (línea 282) para compatibilidad con el flujo manual, ahora también protegido por `_semaforoPuerto`.

### 3. Protocolo "F" — Metadatos de Archivo (`classComunicacion.cs:344-367`)

Se agregó el tipo de trama `"F"` para eliminar la necesidad de que el receptor ingrese manualmente el tamaño del archivo.

**Envío** (en `EnviarArchivoIndividual`, línea 205-215):
- Antes de los chunks `"A"`, se envía una trama `"F"` con el formato `"usuario|nombre|tamaño"`
- El usuario remitente se toma de `txtUsuario.Text` y se sincroniza en `Enlace.NombreUsuario`

**Recepción** (en `ProcesandoMetadatosRecepcion`, línea 356):
- Al recibir una trama `"F"`, se parsea `usuario|nombre|tamaño`
- Se crea el archivo como `usuario_nombre` (ej: `Perez_foto.jpg`) para evitar conflictos entre instancias
- Se dispara el evento `MetadatosRecibidos`

**Recepción manual:** Se mantienen los controles `txtArchivoRecepcion`, `txtTamanoArchivo` y `btnCrearArchivo` como método alternativo en `Form1.Designer.cs`.

### 4. Barras de Progreso (`Form1.cs:312-372`, `Form1.Designer.cs`)

**Envío:**
- `lvArchivosSeleccionados` (ListView) muestra los archivos seleccionados con columnas: Archivo, Tamaño (bytes), Progreso
- El evento `Enlace.ProgresoEnvio` actualiza la columna Progreso (ej. "45%")
- Al completar un archivo, `Enlace.ArchivoEnvioCompletado` marca "COMPLETADO" con fondo verde

**Recepción:**
- `pbRecepcionArchivo` (ProgressBar) muestra el progreso de recepción de 0% a 100%
- `lblEstadoRecepcion` (Label) muestra el nombre y tamaño del archivo que se está recibiendo
- El evento `Enlace.ProgresoRecepcion` actualiza ambos controles vía `Invoke`

### 5. Alineación de Mensajes: Propio vs Remoto (`Form1.cs:89-106`)

Método `AgregarMensajeFormateado(string usuario, string mensaje, bool esPropio)`:

| Propio (`esPropio = true`) | Remoto (`esPropio = false`) |
|---|---|
| `SelectionAlignment = Right` | `SelectionAlignment = Left` |
| Fondo verde claro (`220,248,198`) | Fondo gris claro (`230,230,230`) |
| Se muestra a la derecha | Se muestra a la izquierda |

**Detección** (en `MostrandoMensaje`, línea 76-82):
- Se compara el `usuario` del mensaje recibido con `txtUsuario.Text.Trim()`
- Si coinciden → es propio (derecha), si no → es remoto (izquierda)
- Si el mensaje no tiene formato `usuario|mensaje`, se asigna `DefaultRemoteUser = "Remoto"`

### 6. Mensajes de Abajo hacia Arriba (`Form1.cs:105`)

Cada vez que se agrega un mensaje al `rchConversacion` (RichTextBox), se llama a:
```csharp
rchConversacion.ScrollToCaret();
```
Esto asegura que el scroll se mantenga en la parte inferior, mostrando siempre los mensajes más recientes como en WhatsApp o Telegram.

### 7. Selección de Archivos con Diálogo Nativo (`Form1.cs:228-260`)

Se reemplazó el ingreso manual de rutas (`txtArchivoEnvio`) por un `OpenFileDialog`:
- `Multiselect = true` permite seleccionar hasta 5 archivos
- Los archivos se muestran en el `ListView` `lvArchivosSeleccionados`
- El botón `btnEnviarArchivos` envía todos los archivos seleccionados llamando a `Enlace.EnviarArchivos(rutas)`

### 8. Corrección de Bug en Constructor (`classComunicacion.cs:50`)

**Antes:** `procesoEnvioArchivo = new Thread(EscribiendoRecepcionArchivo);` (asignaba el método de recepción al hilo de envío)

**Ahora:** `procesoEnvioArchivo = new Thread(LeyendoTransmitiendoArchivo);` (método correcto de envío)

### 9. Creación Automática del Directorio `C:\REDES1\` (`classComunicacion.cs:116-121`)

Método `AsegurarDirectorioRedes1()`: verifica y crea el directorio `C:\REDES1\` si no existe, usado tanto en envío como en recepción.

---

## Variables Clave del Proyecto

### `classComunicacion.cs`

| Variable | Tipo | Propósito |
|----------|------|-----------|
| `sPuerto` | `SerialPort` | Puerto serie RS-232 |
| `_semaforoPuerto` | `SemaphoreSlim(1,1)` | Exclusión mutua para escritura al puerto |
| `_semaforoEnvios` | `SemaphoreSlim(5,5)` | Límite de 5 envíos simultáneos |
| `_enviosActivos` | `int` | Contador atómico de envíos en curso |
| `tramaCabacera` | `byte[5]` | Cabecera de trama (tipo + longitud) |
| `tramaEnvioBytes` | `byte[1024]` | Buffer de relleno (bytes `@`) |
| `tramaMensajeEnvio` | `byte[1024]` | Buffer para mensaje a enviar |
| `tramaRecepcionMensaje` | `byte[1024]` | Buffer de recepción |
| `tramaEnvioArchivo` | `byte[1024]` | Buffer para datos de archivo |
| `FlujoLecturaArchivo` | `FileStream` | Stream de lectura del archivo a enviar |
| `leyendoTramaArchivoEnvio` | `BinaryReader` | Lector binario del archivo |
| `FlujoEscrituraArchivo` | `FileStream` | Stream de escritura del archivo recibido |
| `EscribiendoTramaArchivoRecepcion` | `BinaryWriter` | Escritor binario del archivo recibido |
| `tamanoArchivo` | `long` | Tamaño total del archivo a enviar |
| `tamanoArchivoRecepcion` | `long` | Tamaño esperado del archivo a recibir |
| `avanceRecepcionArchivo` | `long` | Bytes recibidos acumulados |
| `TAREA` | `string` | Identificador de trama (`"M"`, `"A"`, `"F"`, `"I"`) |

### Eventos

| Evento | Firma | Disparado por |
|--------|-------|---------------|
| `LlegoMensaje` | `miManejador(string)` | `RecibiendoMensaje()` |
| `ProgresoEnvio` | `Action<string, long, long>` | `EnviarArchivoIndividual()` |
| `ArchivoEnvioCompletado` | `Action<string>` | `EnviarArchivoIndividual()` |
| `MetadatosRecibidos` | `Action<string, long>` | `ProcesandoMetadatosRecepcion()` |
| `ProgresoRecepcion` | `Action<long, long>` | `EscribiendoRecepcionArchivo()` |

### `Form1.cs`

| Variable/Control | Tipo | Propósito |
|------------------|------|-----------|
| `Enlace` | `ClassComunicacion` | Instancia de comunicación |
| `rchConversacion` | `RichTextBox` | Área de chat |
| `txtUsuario` | `TextBox` | Nombre del usuario local |
| `lvArchivosSeleccionados` | `ListView` | Lista de archivos a enviar |
| `pbRecepcionArchivo` | `ProgressBar` | Barra de progreso de recepción |
| `lblEstadoRecepcion` | `Label` | Estado de la recepción en curso |
| `MuestraProgresoArchivo` | `AccedeControlProgreso` | Delegado para actualizar progreso en UI |

---

## Cómo Ejecutar

1. Conectar dos puertos COM con un cable null-modem (o usar emulador como com0com)
2. Ejecutar `launch.bat` o compilar con:
   ```
   dotnet build
   ```
3. Iniciar ambas instancias (COMA en COM10, COMB en COM11)
4. Ingresar un nombre de usuario en cada lado
5. Para enviar archivos: click en "SELECCIONAR..." → elegir hasta 5 archivos → click en "ENVIAR ARCHIVOS"
6. La recepción de archivos es automática al recibir tramas "F"

---

## Resumen de Hilos y Sincronización

```
┌─────────────────────────────────────────┐
│         _semaforoEnvios (5 slots)        │
│  ┌──────┐ ┌──────┐ ┌──────┐             │
│  │Hilo 1│ │Hilo 2│ │Hilo 3│ ... (max 5) │
│  └──┬───┘ └──┬───┘ └──┬───┘             │
│     │        │        │                  │
│     └────────┼────────┘                  │
│              ▼                           │
│     _semaforoPuerto (1 slot)             │
│              │                           │
│              ▼                           │
│       sPuerto.Write()                    │
│       (puerto serie RS-232)              │
└─────────────────────────────────────────┘

Mensajes (hebraEnvio) ──► _semaforoPuerto ──► sPuerto.Write()
                          (comparten el mismo semáforo)
```

- Los mensajes y los archivos comparten `_semaforoPuerto`: no hay bloqueo mutuo, uno espera al otro
- El envío de archivos usa `_semaforoEnvios` (5 slots): máximo 5 archivos en paralelo
- La recepción (`SPuerto_DataReceived`) no compite por el puerto porque solo lee; la escritura de archivos recibidos (`EscribiendoRecepcionArchivo`) escribe a `FileStream`, no al puerto serie
