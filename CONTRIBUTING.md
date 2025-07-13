# Guía de Contribución - API de Gestión de Flota 🚗

¡Gracias por colaborar en este proyecto! Para mantener la calidad del código y facilitar el trabajo en equipo, por favor sigue estas reglas:

## 🛑 No trabajar directamente en `main` (ni `master`)
- `main` está protegido. Nadie puede hacer `push` directo.
- Todo cambio debe hacerse desde una rama separada y ser aprobado mediante Pull Request.

## 🧩 Flujo de trabajo recomendado
1. Crear una rama nueva desde `develop` o `main`:
   - `feature/nombre-modulo` → Para funcionalidades nuevas.
   - `bugfix/descripcion-error` → Para arreglos de errores.
   - `hotfix/urgente` → Para arreglos críticos.
   - `test/nombre-prueba` → Para pruebas específicas.

2. Realizar tus cambios localmente.

3. Hacer `commit` y `push` a tu rama.

4. Crear un Pull Request hacia la rama `develop` (o `main`, si ya está validado).

5. Esperar aprobación de al menos un revisor antes de hacer merge.

## ✅ Buenas prácticas
- Escribe mensajes de `commit` claros:  
  Ej: `feat(vehiculos): agregar validación de placa duplicada`

- Asegúrate de que tu código compila y pasa las pruebas (`dotnet test`).

- Si el PR tiene comentarios, **resuélvelos antes de pedir merge**.
