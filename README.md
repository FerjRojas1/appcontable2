# AppContable

Sistema web de gestión impositiva para estudios contables. Importa las ventas y compras de cada cliente desde planillas de **ARCA** y calcula **IVA**, **Ingresos Brutos** y el **resumen de Ganancias**, con un calendario de vencimientos y gráficos por cliente.

> 🚧 **Estado: versión 2 en desarrollo.** Es un remake del sistema que armé como Práctica Profesional Supervisada (PPS). Estoy corrigiendo bugs y rediseñando el flujo y las interfaces. Los datos de la demo son ficticios.

## 🔗 Demo

**https://appcontable-ferrojasdev.runasp.net/**

| Usuario | Contraseña |
|---|---|
| `Visitante` | `v1sit@nT3` |

## 📖 Contexto

Trabajaba en un estudio contable y conocía el flujo real de liquidación de impuestos y gestión de clientes, con mucho trabajo manual. Por eso decidí usar ese conocimiento en mi PPS: el objetivo fue automatizar la clasificación de la información importada desde ARCA, que pasó de llevar aproximadamente **1 hora a 5-10 minutos**.

El proyecto original lo desarrollamos en equipo y yo fui **Project Manager**, además de desarrollar funcionalidades de backend y frontend. Repo de la PPS: [app-impositiva-contable](https://github.com/FerjRojas1/app-impositiva-contable).

## ✨ Funcionalidades

### Usuarios y roles
ABM de usuarios con dos roles:
- **Admin:** accede a la **auditoría** del sistema.
- **Usuario:** accede solo a la **gestión de clientes**.

### Calendario
El inicio muestra un calendario para registrar vencimientos, tareas o cualquier otro evento.

### Clientes
- ABM de clientes con un listado (data table) que muestra los datos básicos.
- Cada cliente tiene su vista de detalle, con un **gráfico anual de compras vs. ventas por mes**.


### Ventas y compras
El ABM se puede hacer de forma **manual** o por **importación masiva desde Excel** (exportado desde ARCA).

La importación **valida las alícuotas**: cada comprobante debe tener una alícuota definida. Si alguno no la tiene, el sistema le pide al usuario que **desglose el IVA** en ese momento.


### Impuestos
Los impuestos están dentro de cada cliente, en el menú de navegación del cliente. Como las ventas y compras ya están cargadas, el sistema tiene lo necesario para calcular cada impuesto.


**IVA**
Toma las ventas y compras del período, calcula los totales de **débito y crédito fiscal** netos y determina el **saldo técnico**. En esa misma vista se cargan retenciones y percepciones para obtener el **saldo final** del período. Se puede guardar el resumen e imprimirlo.


**Ingresos Brutos**
La base imponible ya viene cargada desde el Excel de ventas. Solo resta definir la alícuota y cargar manualmente las deducciones correspondientes.

**Resumen de Ganancias**
Totales netos de ventas y compras del año, separados por período (con desglose por alícuota de 27 %, 21 % y 10,5 %).


## 🛠️ Tecnologías

- **Backend:** C# / ASP.NET MVC (.NET 8)
- **Base de datos:** SQL Server con Entity Framework
- **Frontend:** plantilla Bootstrap AdminLTE + HTML, CSS y JavaScript
- **Tests:** proyecto de tests automatizados
- **Control de versiones:** Git / GitHub

## 🏗️ Arquitectura

Arquitectura MVC (Models, Views, Controllers), con la lógica de negocio separada en una capa de servicios (`ServiciosEC`) y persistencia en SQL Server.

## 🗺️ Objetivos del remake

- [x] Corrección de bugs del sistema original.
- [x] Mejora de las interfaces.
- [x] Demo desplegada online.
- [ ] Mejorar el flujo para que sea más intuitivo y fácil de usar.
- [ ] Sumar **gráficos y KPIs** por impuesto, para ver de un vistazo cómo viene el año de cada cliente.


## 👨‍💻 Autor

**Fernando Rojas** — Full Stack Developer

---
*Proyecto original desarrollado en equipo como PPS (Christian Fidelio, Pablo Cejas, Miguel Cejas, Tadeo Bodetto y Fernando Rojas). Esta versión es una reescritura y mejora personal en curso.*
