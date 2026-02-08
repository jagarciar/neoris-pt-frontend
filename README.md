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
- Validación local del JWT sin inyección de dependencias

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
        container.RegisterInstance(ApiClientService.Instance);

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
    <add key="ApiBaseUrl" value="http://localhost:5000/api/"/>
</appSettings>
```

La URL debe terminar con `/` para construir endpoints relativos correctamente. Si el backend corre en otro puerto, actualiza esta configuración.

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

        var response = await httpClient.PostAsync("v1/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);
            
            // Guardar token en sesión
            Session["Token"] = loginResponse.AccessToken;
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
        
        var response = await httpClient.GetAsync("v1/autores");
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
        
        var response = await httpClient.PostAsync("v1/autores", content);
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
        
        var response = await httpClient.PutAsync($"v1/autores/{id}", content);
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
        
        var response = await httpClient.DeleteAsync($"v1/autores/{id}");
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
    var response = await httpClient.GetAsync("v1/autores");
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
