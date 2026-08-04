# Auditoría Avance 5 — Gestión de Pedidos (FastBurger)

> Instrucciones para el agente: revisa el código REAL del proyecto (controllers, services, views) 
> contra cada criterio. Para cada fila responde: ✅ Cumple / ⚠️ Parcial / ❌ Falta, 
> con el archivo y línea donde lo verificaste. No asumas que algo está hecho sin leer el código.

---

## BLOQUE 1: Historial de Pedidos

- [ ] Datos precargados: más de 3 pedidos de prueba en el seed
- [ ] Diseño estructurado, ordenado, intuitivo
- [ ] Acciones (botones/enlaces) fáciles de encontrar
- [ ] El usuario logueado se establece en la LÓGICA (backend/sesión), NO se selecciona en la interfaz
- [ ] Cliente ve TODOS sus pedidos, ordenados por fecha
- [ ] Cliente: cada pedido tiene un enlace para acceder a su detalle
- [ ] Encargado/Admin ve TODOS los pedidos, ordenados por fecha
- [ ] Encargado/Admin: cada pedido tiene un enlace para acceder a su detalle
- [ ] Filtro por fecha funcional
- [ ] Filtro por estado funcional
- [ ] Todos los valores de campos son entendibles (no IDs crudos, no enums sin traducir)
- [ ] Nombres de campos son representativos (no "Estado1", "Campo2", etc.)
- [ ] Formato de moneda con símbolo (₡, $, etc.) en todos los valores monetarios
- [ ] Un solo idioma en toda la vista
- [ ] Sin errores de ortografía/redacción
- [ ] La aplicación ejecuta sin errores en esta pantalla

---

## BLOQUE 2: Detalle de Pedido

- [ ] Datos precargados: más de 3 registros disponibles para probar
- [ ] Diseño estructurado, ordenado, intuitivo
- [ ] Muestra toda la información del pedido seleccionado
- [ ] Formato de factura: encabezado, detalle y totales claramente distinguibles
- [ ] Encabezado muestra: fecha, cliente, encargado, método de entrega, método de pago, estado
- [ ] Encabezado - cliente: al menos 2 campos que lo identifiquen
- [ ] Encabezado - muestra información del encargado
- [ ] Encabezado - muestra método de entrega
- [ ] Encabezado - muestra método de pago
- [ ] Encabezado - muestra estado del pedido
- [ ] Detalle: varias líneas con nombre, precio, cantidad, subtotal, impuesto, observaciones
- [ ] Totales: total sin impuesto Y total con impuesto (incluyendo envío si aplica)
- [ ] Formato de moneda correcto en todos los valores
- [ ] Formato de fecha/hora correcto según idioma de la app
- [ ] Valores y nombres de campos entendibles
- [ ] Un solo idioma, sin errores de ortografía
- [ ] La aplicación ejecuta sin errores en esta pantalla

---

## BLOQUE 3: Gestión de Pedido (Registrar)

### Generales
- [ ] Datos precargados: más de 3 registros para pruebas
- [ ] Diseño estructurado, ordenado, intuitivo; acciones fáciles de encontrar

### Contador de cantidad de compra
- [ ] Aparece en el encabezado de la app
- [ ] Es la sumatoria de cantidades de productos + combos agregados
- [ ] **Se actualiza automáticamente (sin recargar página)** ⚠️ punto crítico ya identificado

### Formulario de pedido
- [ ] Formato de factura: encabezado + tabla de detalle
- [ ] Encabezado muestra fecha actual con formato correcto
- [ ] Encabezado - cliente: 2 campos que lo identifiquen
- [ ] Si el CLIENTE está logueado: sus datos se autocompletan, NO editables, NO se selecciona en la interfaz
- [ ] Si el ENCARGADO está logueado: puede seleccionar un cliente de una lista
- [ ] La lista de selección de cliente NO muestra ambos campos identificadores a la vez (evitar saturar el dropdown)
- [ ] Al seleccionar un cliente de la lista, su información detallada se muestra A LA PAR (aparte, visible), no solo en el dropdown
- [ ] Muestra el nombre del encargado que registra, no editable
- [ ] Si el encargado está logueado: sus datos se autocompletan, no editables, no se selecciona en interfaz
- [ ] Si el usuario logueado es CLIENTE, no se solicitan datos de encargado
- [ ] Se puede seleccionar método de entrega
- [ ] Si es entrega a domicilio: pide dirección Y agrega costo de envío al total
- [ ] Estado del pedido no editable, se actualiza según reglas del sistema
- [ ] Permite registrar varios productos en un mismo pedido
- [ ] Permite registrar varios combos en un mismo pedido
- [ ] Permite mezclar productos y combos en un mismo pedido
- [ ] Al seleccionar producto o combo, se muestra el precio automáticamente (ambos casos, no solo uno)
- [ ] Se puede especificar y cambiar la cantidad por línea
- [ ] Validación de cantidad: solo acepta números, CON mensaje de notificación
- [ ] Subtotal y total se actualizan automáticamente y en tiempo real (sin recargar)
- [ ] Subtotal correcto = precio × cantidad (funciona para producto Y combo)
- [ ] Impuesto calculado y mostrado por línea
- [ ] Total sin impuestos = suma de subtotales
- [ ] Total con impuestos = suma de subtotales + impuestos
- [ ] Botón de borrar elimina la línea correspondiente
- [ ] Cantidad en 0 borra la línea, PERO dejar el campo vacío NO debe borrarla
- [ ] Cada línea de detalle (producto o combo) tiene su PROPIO campo de observaciones — NO es un campo único de observaciones generales para todo el pedido

### Pago
- [ ] Métodos: tarjeta de crédito, débito Y efectivo (los 3)
- [ ] Pago con tarjeta: simula con campos básicos claros
- [ ] Pago con efectivo: pide monto con el que paga y calcula el vuelto automáticamente

### Registro final
- [ ] Se puede registrar el pedido completo con todos los aspectos y valores indicados por el usuario (nada hardcodeado/predeterminado)
- [ ] Al registrar, se notifica al usuario
- [ ] Al registrar, el ESTADO del pedido se actualiza automáticamente (ej. a "Pendiente de pago" u otro estado inicial correspondiente, no editable manualmente)
- [ ] Los estados usados a lo largo del flujo son consistentes con algo como: Pendiente de pago, Aceptada, Preparación, Procesando, Entregada

### Transversales (aplican a las 3 pantallas)
- [ ] Formato de moneda correcto en todos los valores
- [ ] Formato de fecha/hora correcto
- [ ] Valores y nombres de campos entendibles
- [ ] Un solo idioma, sin errores de ortografía
- [ ] **Sin recarga de página innecesaria — flujos asíncronos (fetch/AJAX)**
- [ ] La aplicación ejecuta sin errores
