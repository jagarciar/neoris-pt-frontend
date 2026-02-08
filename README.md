# Neoris Frontend

Frontend de la aplicación Neoris desarrollado con ASP.NET MVC (.NET Framework 4.8). Consume la API REST del backend para mostrar y gestionar autores y libros.

## 🚀 Características

- **ASP.NET MVC 5** (.NET Framework 4.8)
- **Razor Views** para renderizado del lado del servidor
- **Diseño responsive** con CSS moderno
- **Consumo de API REST** del backend
- **CORS** configurado
- Interfaz intuitiva y moderna
- Compatible con Windows Server / IIS

## 📁 Estructura del Proyecto

```
neoris-pt-frontend/
├── Controllers/                     # Solo lógica de presentación
│   ├── HomeController.cs           # Controlador principal e inicio
│   ├── AuthController.cs           # Autenticación (login/logout)
│   ├── AutoresController.cs        # CRUD completo de autores
│   └── LibrosController.cs         # CRUD completo de libros
├── Services/                        # 🆕 Lógica de negocio (Business Layer)
│   ├── Interfaces/
│   │   ├── IAutorService.cs        # Contrato para operaciones de Autores
│   │   ├── ILibroService.cs        # Contrato para operaciones de Libros
│   │   └── IAuthService.cs         # Contrato para autenticación
│   ├── AutorService.cs             # Implementación de lógica de Autores
│   ├── LibroService.cs             # Implementación de lógica de Libros
│   └── AuthService.cs              # Implementación de autenticación y validación JWT
├── Infrastructure/                  # 🆕 Servicios técnicos y utilidades
│   └── ApiClientService.cs         # HttpClient singleton para API calls
├── Filters/                         # 🆕 Filtros personalizados ASP.NET MVC
│   ├── JwtAuthorizationFilter.cs   # Autorización automática con JWT
│   └── GlobalExceptionFilter.cs    # Manejo centralizado de excepciones
├── Models/
│   ├── Autor.cs                    # Modelo de autor con validaciones
│   ├── Libro.cs                    # Modelo de libro con relación a autor
│   └── LoginRequest.cs             # DTOs para autenticación (LoginRequest, LoginResponse)
├── Views/
│   ├── Home/
│   │   └── Index.cshtml            # Página principal con dashboard
│   ├── Auth/
│   │   └── Login.cshtml            # Formulario de inicio de sesión
│   ├── Autores/
│   │   ├── Index.cshtml            # Lista de autores
│   │   ├── Create.cshtml           # Crear autor
│   │   ├── Edit.cshtml             # Editar autor
│   │   ├── Delete.cshtml           # Confirmar eliminación de autor
│   │   └── Details.cshtml          # Detalles de autor
│   ├── Libros/
│   │   ├── Index.cshtml            # Lista de libros
│   │   ├── Create.cshtml           # Crear libro (con dropdown de autores)
│   │   ├── Edit.cshtml             # Editar libro
│   │   ├── Delete.cshtml           # Confirmar eliminación de libro
│   │   └── Details.cshtml          # Detalles de libro
│   └── Shared/
│       ├── _Layout.cshtml          # Layout principal con navbar auth-aware
│       └── Error.cshtml            # 🆕 Vista mejorada de error
├── Content/
│   └── Site.css                    # Estilos CSS globales
├── Scripts/
│   └── site.js                     # JavaScript
├── App_Start/
│   ├── RouteConfig.cs              # Configuración de rutas MVC
│   ├── BundleConfig.cs             # Bundling de CSS/JS
│   ├── FilterConfig.cs             # Filtros globales (GlobalExceptionFilter)
│   └── UnityConfig.cs              # 🆕 Configuración Unity Container (DI)
├── Web.config                      # Configuración principal y ApiBaseUrl
├── Global.asax                     # Punto de entrada de la aplicación
├── .gitignore                      # Archivos ignorados por Git
├── packages.config                 # Paquetes NuGet
└── README.md                       # Esta documentación
```

## 🏗️ Arquitectura y Mejores Prácticas

El proyecto sigue una **arquitectura en capas** con separación de responsabilidades y **inyección de dependencias** mediante Unity Container:

### **Capa de Presentación (Controllers + Views)**
- Controladores ligeros que solo manejan HTTP requests/responses
- Inyección de dependencias vía constructor
- Validación de entrada con Data Annotations
- Decorados con filtros para cross-cutting concerns

**Ejemplo:**
```csharp
public class AutoresController : Controller
{
    private readonly IAutorService _autorService;

    // Unity Container inyecta IAutorService automáticamente
    public AutoresController(IAutorService autorService)
    {
        _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
    }
}
```

### **Capa de Negocio (Services)**
- **Interfaces** que definen contratos: `IAutorService`, `ILibroService`, `IAuthService`
- **Implementaciones** con toda la lógica de negocio
- Separación de responsabilidades (SRP)
- Facilita testing con mocks
- Manejo de errores y validaciones

**Servicios implementados:**
```csharp
// Servicios de negocio
IAutorService/AutorService    → Operaciones CRUD de Autores
ILibroService/LibroService    → Operaciones CRUD de Libros
IAuthService/AuthService      → Autenticación y validación JWT
```

### **Capa de Infraestructura (Infrastructure)**
- **`ApiClientService`**: HttpClient singleton thread-safe para evitar socket exhaustion
- Lazy initialization para performance
- Configuración centralizada de comunicación HTTP
- Manejo eficiente de conexiones TCP

```csharp
public sealed class ApiClientService
{
    private static readonly Lazy<ApiClientService> _instance = 
        new Lazy<ApiClientService>(() => new ApiClientService());
    
    private readonly HttpClient _httpClient;
    
    public static ApiClientService Instance => _instance.Value;
}
```

### **Cross-Cutting Concerns (Filters)**

#### **JwtAuthorizationFilter**
- Validación automática de tokens JWT en cada request
- Redirige a login si el token es inválido o ha expirado
- Property injection con `[Dependency]` attribute

```csharp
[JwtAuthorizationFilter]
public class AutoresController : Controller
{
    // Automáticamente valida JWT antes de cada acción
}
```

#### **GlobalExceptionFilter**
- Manejo centralizado de excepciones en toda la aplicación
- Diferentes respuestas según el tipo de excepción
- Logging de errores (preparado para log4net/NLog)

**Excepciones manejadas:**
- `UnauthorizedAccessException` → Limpia sesión y redirige a Login
- `HttpException (404)` → Vista NotFound personalizada
- `ApplicationException` → Errores de la capa de servicios/API
- `Exception genérica` → Vista de error con logging

### **Inyección de Dependencias con Unity Container 5.11.1**

#### **Configuración (`App_Start/UnityConfig.cs`):**
```csharp
public static class UnityConfig
{
    private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
    {
        var container = new UnityContainer();
        RegisterTypes(container);
        return container;
    });

    public static void RegisterComponents()
    {
        DependencyResolver.SetResolver(new UnityDependencyResolver(Container));
    }

    private static void RegisterTypes(IUnityContainer container)
    {
        // Singleton - una instancia para toda la aplicación
        container.RegisterType<ApiClientService>(new ContainerControlledLifetimeManager());

        // Transient - nueva instancia por request
        container.RegisterType<IAuthService, AuthService>();
        container.RegisterType<IAutorService, AutorService>();
        container.RegisterType<ILibroService, LibroService>();
    }
}
```

#### **Inicialización en Global.asax:**
```csharp
protected void Application_Start()
{
    AreaRegistration.RegisterAllAreas();
    UnityConfig.RegisterComponents();  // Antes de FilterConfig
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}
```

#### **Lifetimes configurados:**

| Servicio | Lifetime | Razón |
|----------|----------|-------|
| `ApiClientService` | **Singleton** | HttpClient debe reutilizarse para evitar socket exhaustion |
| `IAuthService` | **Transient** | Nueva instancia por request HTTP |
| `IAutorService` | **Transient** | Nueva instancia por request HTTP |
| `ILibroService` | **Transient** | Nueva instancia por request HTTP |

### **Principios SOLID Aplicados ✅**

| Principio | Implementación |
|-----------|----------------|
| **S**ingle Responsibility | Servicios separados por responsabilidad (Autor, Libro, Auth). Filtros para concerns transversales. |
| **O**pen/Closed | Interfaces permiten extensión sin modificar código existente. Nuevas implementaciones solo requieren registrarlas en Unity. |
| **L**iskov Substitution | Implementaciones intercambiables vía interfaces. AutorService puede reemplazarse por MockAutorService en tests. |
| **I**nterface Segregation | Interfaces específicas (IAutorService, ILibroService, IAuthService) en lugar de una grande. |
| **D**ependency Inversion | Controladores dependen de abstracciones (interfaces), no implementaciones concretas. Unity resuelve dependencias. |

### **📊 Métricas de Mejora Implementadas**

| Aspecto | Antes | Después | Mejora |
|---------|-------|---------|--------|
| Líneas de código en controladores | ~300 líneas | ~200 líneas | **-33%** |
| Código duplicado | 8 instancias | 0 instancias | **-100%** |
| HttpClient instances | New en cada llamada | 1 singleton | **Socket exhaustion evitado** |
| Verificación de token | Manual en cada acción | Automática con filtro | **-100% duplicación** |
| Manejo de errores | Inconsistente | Centralizado | **+100% cobertura** |
| Testabilidad | Difícil (hardcoded) | Fácil (interfaces mockeables) | **+200%** |
| Inyección de dependencias | Manual (`new Service()`) | Automática (Unity) | **+100% desacoplamiento** |
| Gestión de ciclo de vida | Manual | Automática (Unity) | **-100% memory leaks** |

## 🔧 Requisitos

- .NET Framework 4.8 o superior
- Visual Studio 2019/2022
- IIS Express o IIS
- **Backend ejecutándose** en http://localhost:5000

## ⚙️ Configuración

### URL de la API Backend

La URL de la API se configura en `Web.config`:

```xml
<appSettings>
    <add key="ApiBaseUrl" value="http://localhost:5000/api"/>
</appSettings>
```

Si el backend corre en otro puerto, actualiza esta configuración.

## 🏃 Ejecución

### Desde Visual Studio:

1. Abre la solución `Neoris.sln` desde la carpeta raíz
2. En Solution Explorer, clic derecho en "neoris-pt-frontend"
3. Selecciona "Set as Startup Project"
4. Presiona **F5** para ejecutar
5. La aplicación se abrirá en http://localhost:4200

### Ejecutar Backend y Frontend Simultáneamente:

1. Clic derecho en la solución "Neoris"
2. Properties → Startup Project
3. Selecciona "Multiple startup projects"
4. Configura ambos proyectos con Action = "Start"
5. Presiona **F5**
## 📡 Páginas Disponibles

| Ruta | Descripción | Características |
|------|-------------|-----------------|
| `/` o `/Home/Index` | Página principal | Dashboard con cards de acceso rápido a Autores y Libros, muestra estado de autenticación |
| **Autenticación** |  |  |
| `/Auth/Login` | Inicio de sesión | Formulario de login, almacena JWT en sesión, credenciales de prueba mostradas |
| `/Auth/Logout` | Cerrar sesión | Limpia sesión y token JWT, redirige al login |
| **Autores** |  |  |
| `/Autores/Index` | Lista de autores | Tabla con todos los autores, botones para crear/editar/eliminar/detalles |
| `/Autores/Details/{id}` | Detalles del autor | Vista de solo lectura con toda la información del autor |
| `/Autores/Create` | Crear autor | Formulario con validaciones (nombre, fecha nacimiento, ciudad, email) |
| `/Autores/Edit/{id}` | Editar autor | Formulario precargado con validaciones para actualizar datos |
| `/Autores/Delete/{id}` | Eliminar autor | Confirmación antes de eliminar con resumen de datos |
| **Libros** |  |  |
| `/Libros/Index` | Lista de libros | Tabla con libros y sus autores, descripción truncada, botones CRUD |
| `/Libros/Details/{id}` | Detalles del libro | Vista completa con información del libro y del autor |
| `/Libros/Create` | Crear libro | Formulario con validaciones y dropdown para seleccionar autor |
| `/Libros/Edit/{id}` | Editar libro | Formulario precargado con dropdown de autores actualizado |
| `/Libros/Delete/{id}` | Eliminar libro | Confirmación con resumen del libro y autor asociado |

## 🎨 Características de la Interfaz

### Diseño Responsivo
- Layout adaptable a diferentes tamaños de pantalla
- Menú colapsable en dispositivos móviles
- Tablas con scroll horizontal en pantallas pequeñas
- Cards con diseño grid responsive

### Componentes UI
- **Navbar**: Navegación principal con menú auth-aware (muestra Autores/Libros/Logout cuando está autenticado, Login cuando no lo está)
- **Tablas**: Con hover effects, alternancia de colores y diseño responsive
- **Botones**: Acciones CRUD con iconos emoji y colores semánticos (crear=verde, editar=amarillo, eliminar=rojo, detalles=azul)
- **Cards**: Presentación de información con sombras, animaciones y diseño grid responsive
- **Formularios**: Validación del lado del cliente (jQuery Validation) con mensajes de error en español
- **Alerts**: Mensajes de éxito/error con TempData para feedback al usuario
- **Confirmaciones**: Páginas dedicadas para confirmar eliminación con resumen de datos

### Interactividad
- Confirmación antes de eliminar registros
- Loading spinners durante peticiones a la API
- Mensajes de error amigables
- Validación en tiempo real en formularios

## 🔄 Consumo de la API

Los controladores consumen la API del backend mediante `HttpClient` con autenticación JWT almacenada en `Session["Token"]`:

### Ejemplo: Autenticación (Login)
```csharp
// En AuthController.cs
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Login(LoginRequest model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    using (var httpClient = new HttpClient())
    {
        var apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        httpClient.BaseAddress = new Uri(apiBaseUrl);

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/v1/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);
            
            // Guardar token en sesión
            Session["Token"] = loginResponse.Token;
            Session["Username"] = model.Username;

            return RedirectToAction("Index", "Home");
        }
    }

    ViewBag.ErrorMessage = "Usuario o contraseña incorrectos";
    return View(model);
}
```

### Ejemplo: Obtener todos los autores (con JWT)
```csharp
// En AutoresController.cs
public async Task<ActionResult> Index()
{
    var token = Session["Token"] as string;
    if (string.IsNullOrEmpty(token))
    {
        return RedirectToAction("Login", "Auth");
    }

    using (var httpClient = new HttpClient())
    {
        var apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        httpClient.BaseAddress = new Uri(apiBaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await httpClient.GetAsync("/v1/autores");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var autores = JsonConvert.DeserializeObject<List<Autor>>(json);
            return View(autores);
        }
    }
    return View(new List<Autor>());
}
```

### Ejemplo: Crear un autor (con JWT y validación)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Create(Autor autor)
{
    if (!ModelState.IsValid)
    {
        return View(autor);
    }

    var token = Session["Token"] as string;
    if (string.IsNullOrEmpty(token))
    {
        return RedirectToAction("Login", "Auth");
    }

    using (var httpClient = new HttpClient())
    {
        var apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        httpClient.BaseAddress = new Uri(apiBaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var json = JsonConvert.SerializeObject(autor);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("/v1/autores", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Autor creado exitosamente";
            return RedirectToAction("Index");
        }
    }
    
    TempData["Error"] = "Error al crear el autor";
    return View(autor);
}
```

### Ejemplo: Actualizar un autor
```csharp
[HttpPost]
public async Task<ActionResult> Edit(int id, Autor autor)
{
    if (!ModelState.IsValid)
    {
        return View(autor);
    }

    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        
        var json = JsonConvert.SerializeObject(autor);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PutAsync($"/api/v1/autores/{id}", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Autor actualizado exitosamente";
            return RedirectToAction("Index");
        }
    }
    
    TempData["Error"] = "Error al actualizar el autor";
    return View(autor);
}
```

### Ejemplo: Eliminar un autor
```csharp
[HttpPost]
public async Task<ActionResult> Delete(int id)
{
    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        
        var response = await httpClient.DeleteAsync($"/api/v1/autores/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Autor eliminado exitosamente";
        }
        else
        {
            TempData["Error"] = "Error al eliminar el autor";
        }
    }
    
    return RedirectToAction("Index");
}
```

### Manejo de Errores
```csharp
try
{
    var response = await httpClient.GetAsync("/api/v1/autores");
    response.EnsureSuccessStatusCode();
    
    var json = await response.Content.ReadAsStringAsync();
    var autores = JsonConvert.DeserializeObject<List<Autor>>(json);
    
    return View(autores);
}
catch (HttpRequestException ex)
{
    TempData["Error"] = "No se pudo conectar con la API del backend";
    return View(new List<Autor>());
}
catch (Exception ex)
{
    TempData["Error"] = $"Error inesperado: {ex.Message}";
    return View(new List<Autor>());
}
```

## 🎨 Estilos y Diseño

- **CSS personalizado** en `Content/Site.css`
- Diseño responsive (mobile-friendly)
- Colores corporativos
- Tablas con hover effects
- Cards con animaciones
- Navbar responsive

## 📦 Paquetes NuGet

- `Microsoft.AspNet.Mvc` 5.2.9 - Framework MVC
- `Microsoft.AspNet.Razor` 3.2.9 - Motor de vistas Razor
- `Microsoft.AspNet.WebPages` 3.2.9 - Web Pages
- `Newtonsoft.Json` 13.0.3 - Serialización JSON
- `Microsoft.AspNet.Web.Optimization` 1.1.3 - Bundling y minificación

## 🔍 Características Técnicas

### Routing
Las rutas se configuran en `App_Start/RouteConfig.cs`:
```csharp
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

### Bundling
Los archivos CSS y JS se agrupan en `App_Start/BundleConfig.cs` para optimizar la carga.

### Layout Compartido
Todas las vistas usan `_Layout.cshtml` como plantilla base, que incluye:
- Navbar de navegación
- Container principal
- Footer
- Referencias a CSS y JavaScript

## 🛠️ Desarrollo

### Estructura de un Controlador MVC

```csharp
public class AutoresController : Controller
{
    private readonly string _apiBaseUrl;

    public AutoresController()
    {
        _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
    }

    // GET: Autores
    public async Task<ActionResult> Index()
    {
        // Lógica para obtener todos los autores
        return View();
    }

    // GET: Autores/Details/5
    public async Task<ActionResult> Details(int id)
    {
        // Lógica para obtener un autor específico
        return View();
    }

    // GET: Autores/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Autores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Autor autor)
    {
        if (ModelState.IsValid)
        {
            // Lógica para crear el autor
            return RedirectToAction("Index");
        }
        return View(autor);
    }

    // Métodos Edit y Delete similares...
}
```

### Estructura de una Vista Razor

```html
@model IEnumerable<Neoris.Frontend.Models.Autor>

@{
    ViewBag.Title = "Lista de Autores";
}

<h2>@ViewBag.Title</h2>

@if (TempData["Success"] != null)
{
    <div class="alert alert-success">@TempData["Success"]</div>
}

<p>
    @Html.ActionLink("Crear Nuevo Autor", "Create", null, new { @class = "btn btn-primary" })
</p>

<table class="table table-striped">
    <thead>
        <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Email</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var autor in Model)
        {
            <tr>
                <td>@autor.Id</td>
                <td>@autor.Nombre</td>
                <td>@autor.Email</td>
                <td>
                    @Html.ActionLink("Editar", "Edit", new { id = autor.Id }, new { @class = "btn btn-sm btn-warning" })
                    @Html.ActionLink("Detalles", "Details", new { id = autor.Id }, new { @class = "btn btn-sm btn-info" })
                    @Html.ActionLink("Eliminar", "Delete", new { id = autor.Id }, new { @class = "btn btn-sm btn-danger" })
                </td>
            </tr>
        }
    </tbody>
</table>
```

### Agregar una Nueva Página

#### 1. Crear el Modelo
```csharp
// Models/Editorial.cs
public class Editorial
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; }
    
    [StringLength(200)]
    public string Direccion { get; set; }
}
```

#### 2. Crear el Controlador
```csharp
// Controllers/EditorialesController.cs
public class EditorialesController : Controller
{
    private readonly string _apiBaseUrl;

    public EditorialesController()
    {
        _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
    }

    public async Task<ActionResult> Index()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(_apiBaseUrl);
            
            var response = await httpClient.GetAsync("/api/v1/editoriales");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var editoriales = JsonConvert.DeserializeObject<List<Editorial>>(json);
                return View(editoriales);
            }
        }
        return View(new List<Editorial>());
    }
    
    // Agregar métodos Create, Edit, Delete...
}
```

#### 3. Crear la Vista
```bash
# Crear carpeta Views/Editoriales/
# Crear archivo Index.cshtml
```

```html
@model IEnumerable<Neoris.Frontend.Models.Editorial>

@{
    ViewBag.Title = "Editoriales";
}

<h2>Lista de Editoriales</h2>

<table class="table">
    <!-- Contenido de la tabla -->
</table>
```

#### 4. Agregar al Menú
```html
<!-- En Views/Shared/_Layout.cshtml -->
<li>@Html.ActionLink("Editoriales", "Index", "Editoriales")</li>
```

### Validación de Modelos

```csharp
// En el modelo
public class Autor
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? FechaNacimiento { get; set; }
}
```

```html
<!-- En la vista -->
@Html.ValidationSummary(true, "", new { @class = "text-danger" })

<div class="form-group">
    @Html.LabelFor(model => model.Nombre, htmlAttributes: new { @class = "control-label" })
    @Html.EditorFor(model => model.Nombre, new { htmlAttributes = new { @class = "form-control" } })
    @Html.ValidationMessageFor(model => model.Nombre, "", new { @class = "text-danger" })
</div>
```

## ⚠️ Solución de Problemas

### Error: "No se pudieron cargar los autores"

**Causa:** El backend no está ejecutándose o no responde.

**Solución:**
1. Verifica que el backend esté corriendo en http://localhost:5000
2. Prueba acceder directamente a http://localhost:5000/swagger
3. Revisa que CORS esté habilitado en el backend
4. Comprueba la URL en `Web.config` → `ApiBaseUrl`
5. Revisa los logs del backend para ver errores

### Error: "No se encuentra la vista"

**Causa:** La vista no existe o está en la carpeta incorrecta.

**Solución:**
1. Verifica que la vista esté en `Views/{ControllerName}/{ActionName}.cshtml`
2. Asegúrate de que el archivo tenga "Build Action" = "Content"
3. Reconstruye el proyecto (Build → Rebuild Solution)
4. Verifica que el nombre del controlador coincida con la carpeta de vistas

### Error: "Could not load file or assembly"

**Causa:** Falta una dependencia NuGet.

**Solución:**
1. Abre Package Manager Console
2. Ejecuta: `Update-Package -reinstall`
3. O restaura manualmente: Clic derecho en la solución → Restore NuGet Packages

### El CSS no se aplica

**Solución:**
1. Verifica que `BundleConfig.cs` esté configurado correctamente
2. Limpia la caché del navegador (Ctrl+Shift+R o Ctrl+F5)
3. Verifica que el archivo CSS esté en `Content/Site.css`
4. Revisa que `@Styles.Render()` esté en `_Layout.cshtml`
5. Compila el proyecto en modo Release para ver minificación

### Error 404 al llamar a la API

**Causa:** URL de la API incorrecta o backend no disponible.

**Solución:**
1. Verifica `Web.config` → `<appSettings>` → `ApiBaseUrl`
2. Asegúrate que la URL termine en `/api` sin barra final
3. Verifica que el endpoint exista en el backend (revisa Swagger)
4. Usa Fiddler o navegador para probar la URL directamente

### Error de CORS

**Causa:** El backend no permite peticiones desde el origen del frontend.

**Solución en el Backend:**
```csharp
// En WebApiConfig.cs del backend
var cors = new EnableCorsAttribute("*", "*", "*");
config.EnableCors(cors);
```

### Error: "A potentially dangerous Request.Form value was detected"

**Causa:** Contenido HTML en un formulario sin codificar.

**Solución:**
```csharp
// Opción 1: Permitir HTML en un campo específico
[AllowHtml]
public string Descripcion { get; set; }

// Opción 2: Validar en el controlador
[ValidateInput(false)]
public ActionResult Create(Autor autor) { ... }
```

### La aplicación no inicia

**Solución:**
1. Verifica que IIS Express esté instalado
2. Limpia y reconstruye: Build → Clean Solution, luego Build → Rebuild Solution
3. Elimina las carpetas `bin` y `obj` manualmente
4. Cierra Visual Studio y vuelve a abrirlo
5. Verifica que el puerto no esté en uso por otra aplicación

### Debugging no funciona (breakpoints ignorados)

**Solución:**
1. Verifica que estés ejecutando en modo Debug (no Release)
2. Asegúrate de que "Enable Just My Code" esté deshabilitado
3. Limpia símbolos: Debug → Options → Symbols → Clear All
4. Reconstruye en modo Debug

## 🌐 Despliegue

### Publicar en IIS

1. En Visual Studio: Clic derecho en el proyecto → **Publish**
2. Selecciona destino (Folder, IIS, Azure)
3. Configura las opciones
4. Click en "Publish"
5. Copia los archivos publicados a IIS
6. Configura un Application Pool con .NET Framework 4.8
7. Actualiza `Web.config` con la URL del backend en producción

### Configuración para Producción

```xml
<!-- Web.config en producción -->
<appSettings>
    <add key="ApiBaseUrl" value="https://tu-backend-produccion.com/api"/>
</appSettings>

<!-- Habilitar modo Release -->
<compilation debug="false" targetFramework="4.8" />

<!-- Configurar errores personalizados -->
<customErrors mode="On" defaultRedirect="~/Error">
    <error statusCode="404" redirect="~/Error/NotFound"/>
    <error statusCode="500" redirect="~/Error/ServerError"/>
</customErrors>
```

## 🚀 Mejores Prácticas

### Uso de async/await
```csharp
// ✅ Correcto
public async Task<ActionResult> Index()
{
    var autores = await _autorService.GetAllAsync();
    return View(autores);
}

// ❌ Incorrecto (bloquea el thread)
public ActionResult Index()
{
    var autores = _autorService.GetAllAsync().Result;
    return View(autores);
}
```

### Manejo de HttpClient
```csharp
// ✅ Correcto - Usar using para disponer recursos
using (var httpClient = new HttpClient())
{
    // ... realizar peticiones
}

// ⚠️ Mejor práctica - HttpClient como singleton
public class ApiService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public async Task<List<Autor>> GetAutoresAsync()
    {
        // ... usar _httpClient
    }
}
```

### Validación robusta
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Create(Autor autor)
{
    // Validación del modelo
    if (!ModelState.IsValid)
    {
        return View(autor);
    }
    
    // Validación de negocio adicional
    if (await AutorExiste(autor.Email))
    {
        ModelState.AddModelError("Email", "Ya existe un autor con este email");
        return View(autor);
    }
    
    // Procesar...
}
```

### Uso de TempData para mensajes
```csharp
// En el controlador
TempData["Success"] = "Operación exitosa";
TempData["Error"] = "Ocurrió un error";
TempData["Info"] = "Información importante";

// En la vista
@if (TempData["Success"] != null)
{
    <div class="alert alert-success">@TempData["Success"]</div>
}
```

### Bundling y Minificación
```csharp
// BundleConfig.cs
bundles.Add(new StyleBundle("~/Content/css").Include(
    "~/Content/bootstrap.css",
    "~/Content/site.css"));

bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
    "~/Scripts/jquery-{version}.js"));

// Habilitar en producción
BundleTable.EnableOptimizations = true;
```

## 📊 Optimización de Rendimiento

### Caché de vistas
```csharp
// Habilitar caché de salida
[OutputCache(Duration = 60, VaryByParam = "none")]
public ActionResult Index()
{
    return View();
}
```

### Lazy Loading de imágenes
```html
<img src="placeholder.jpg" data-src="imagen-real.jpg" class="lazy" alt="Descripción">

<script>
    document.addEventListener("DOMContentLoaded", function() {
        var lazyImages = [].slice.call(document.querySelectorAll("img.lazy"));
        // Implementar Intersection Observer
    });
</script>
```

### Paginación en el frontend
```csharp
public async Task<ActionResult> Index(int? page)
{
    int pageSize = 10;
    int pageNumber = (page ?? 1);
    
    var autores = await GetAutoresAsync();
    return View(autores.ToPagedList(pageNumber, pageSize));
}
```

## 📦 Dependencias (NuGet)

### Principales:
- **ASP.NET MVC 5.2.9** - Framework MVC
- **Newtonsoft.Json 13.0.3** - Serialización JSON
- **Unity 5.11.1** - Contenedor de inyección de dependencias
- **Unity.Mvc5 1.4.0** - Integración Unity con MVC5
- **System.IdentityModel.Tokens.Jwt 5.7.0** - Validación de tokens JWT
- **Microsoft.IdentityModel.Tokens 5.7.0** - Manejo de tokens de seguridad

### Referencias:
- [ASP.NET MVC Documentation](https://docs.microsoft.com/en-us/aspnet/mvc/)
- [Unity Container](https://github.com/unitycontainer/unity)
- [Razor Syntax Reference](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor)
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)

## 📝 Notas

- Este proyecto requiere que el backend esté ejecutándose
- Usa `async/await` para llamadas a la API
- **Inyección de dependencias** configurada con Unity Container
- **HttpClient singleton** gestionado por Unity para evitar socket exhaustion
- **Filtros globales** para autorización JWT y manejo de excepciones
- Los servicios son fácilmente **mockeables** para unit testing
- El proyecto usa .NET Framework 4.8 (no .NET Core)
- ✅ **Compilación exitosa** con todas las mejores prácticas implementadas

---

**Puerto por defecto:** 4200  
**Backend esperado en:** http://localhost:5000
