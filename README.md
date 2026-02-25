# Client-Server Login PoC (.NET) — Grupo 2

Prueba de concepto (PoC) funcional que demuestra el patrón **Cliente–Servidor** con un flujo de autenticación (login) usando:
- **Servidor**: ASP.NET Core Web API (controladores)
- **Cliente**: .NET Console App (HttpClient)

Este repositorio se entrega como parte de la Tarea II (Patrones Arquitecturales), cumpliendo el requisito de **código funcional**, repositorio público en GitHub y `README.md` con instrucciones detalladas de instalación y ejecución.

---

## 1) Objetivo del PoC

Demostrar en código real:
- Separación de responsabilidades: cliente consume un servicio; servidor valida y decide.
- Comunicación por red (request–response) mediante HTTP.
- Manejo de estado de autenticación mediante **token de sesión** (para el PoC) enviado en cada request.

> Nota: este PoC usa un token simple (GUID) para mantenerlo rápido y didáctico. No pretende ser un sistema de seguridad completo.

---

## 2) Arquitectura (estructura del repo)
/
├─ server/ # Servidor (ASP.NET Core Web API)
│ ├─ Controllers/ # Endpoints HTTP
│ ├─ Models/ # DTOs request/response
│ ├─ Services/ # Lógica simple (UserStore / SessionStore)
│ ├─ Program.cs
│ └─ ...
├─ client/ # Cliente (ConsoleApp)
│ └─ Program.cs
├─ docs/
│ ├─ diagrams/ # Diagramas (Componentes, Despliegue, Secuencia)
│ └─ screenshots/ # Evidencia de ejecución (opcional pero recomendado)
└─ README.md



Esto hace visible el patrón Cliente–Servidor: dos aplicaciones separadas (procesos distintos) comunicándose por HTTP.

---

## 3) Requisitos

### Software
- .NET SDK instalado (recomendado .NET 8 o superior)
- Git (para clonar el repositorio)

### Puertos / URL
- La API corre en HTTPS con un puerto local que verás en consola o en el navegador.
- En nuestro caso de ejemplo: `https://localhost:7215`

> Si tu puerto cambia, solo debes ajustar la `baseUrl` en `client/Program.cs`.

---

## 4) Cómo ejecutar (paso a paso)

### Paso A — Clonar el repositorio
git clone https://github.com/NeoFao/client-server-login-poc.git
cd client-server-login-poc

Paso B — Ejecutar el servidor (API)
En una terminal:
cd server
dotnet restore
dotnet run

Si todo está bien, verás que el servidor está escuchando y podrás abrir Swagger, por ejemplo:
https://localhost:7215/swagger/index.html

Paso C — Ejecutar el cliente (ConsoleApp)
En otra terminal:
cd client
dotnet restore
dotnet run

El cliente realizará automáticamente:

GET /health

POST /auth/login

GET /auth/me usando X-Session-Token

---

5) Credenciales demo
El servidor incluye un usuario de prueba en memoria:

Email: demo@demo.com

Password: Demo123!

Puedes cambiar estas credenciales en server/Services/UserStore.cs.

---

6) Endpoints disponibles (Servidor)
Base URL (ejemplo): https://localhost:7215

Health
GET /health

Respuesta esperada:

200 OK

Body: { "status": "OK" }

Auth — Login
POST /auth/login

Body JSON:
{
  "email": "demo@demo.com",
  "password": "Demo123!"
}

Respuestas típicas:

200 OK + { "token": "..." }

400 BadRequest si falta email/password

401 Unauthorized si credenciales inválidas

Auth — Me (protegido)
GET /auth/me

Header requerido:

X-Session-Token: <token>

Respuestas típicas:

200 OK + { "email": "demo@demo.com" }

401 Unauthorized si falta token o es inválido

---

7) Qué hace el cliente (ConsoleApp)
El cliente representa el rol Cliente del patrón:

Construye requests HTTP.

Envía credenciales al servidor (login).

Recibe un token de sesión.

Repite requests posteriores adjuntando el token para demostrar “estado” (sesión).

Salida esperada (ejemplo):
1) Health check...
Health: 200 OK
{"status":"OK"}

2) Login...
Login: 200 OK
Token: 9d11772b0431477c9fc43d30fa292a07

3) Call /auth/me with X-Session-Token...
Me: 200 OK
{"email":"demo@demo.com"}

---

8) Decisiones arquitectónicas (para defender en evaluación)
¿Por qué token simple y no JWT/Identity?
Es un PoC enfocado en demostrar el patrón Cliente–Servidor rápido y de forma clara.

Reduce complejidad de librerías y configuración.

Deja visible el concepto de “estado de sesión” y cómo se transmite entre requests.

¿Dónde vive la lógica sensible?
En el servidor:

UserStore valida credenciales.

SessionStore crea y valida tokens.

El cliente solo consume el contrato (endpoints + headers) y muestra resultados.

---

9) Seguridad (nota mínima para el PoC)
En sistemas reales, el identificador de sesión/token es un activo sensible.
Debe transmitirse por HTTPS/TLS durante toda la sesión, no solo durante el login, y debe protegerse para evitar secuestro de sesión. 

Este proyecto deshabilita validación estricta del certificado solo en el cliente de consola para facilitar pruebas locales (desarrollo). No usar este enfoque en producción.

---

10) Diagramas
Los diagramas del trabajo se incluyen en:

docs/diagrams/

Recomendado:

Componentes (UML Component)

Despliegue (UML Deployment)

Secuencia del login (UML Sequence)

---

11) Evidencia de ejecución (recomendado)
Para respaldar la funcionalidad del PoC, guardar:

Captura de Swagger con endpoints

Captura de la consola del cliente (flujo completo)

Ubicación:

docs/screenshots/

---

13) Autores
Fabrizio Espinoza Arce

Guillermo Morice Díaz

Yariel Elizondo Jiménez

Curso: Ingeniería del Software I
