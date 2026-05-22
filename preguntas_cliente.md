# Preguntas y Dudas para el Cliente - SportTrack

Este documento compila las dudas de negocio y comportamiento de la aplicación para conversar con el cliente/asociación deportiva.

## 📌 Gestión de Pagos e Inscripciones a Eventos

### 1. Comportamiento ante Atletas Impagos (Propuesta actual: Permitir Inscripción)
* **Pregunta**: Si un atleta no abona su pago correspondiente al evento al que se inscribió (aparece impago al día), momentáneamente hemos decidido **dejarlo inscripto y que aparezca en el sistema**. 
  * ¿Cómo debería visualizarse este atleta en las grillas de inscripciones del Club y de la Federación? ¿Debería tener una advertencia visual destacada (ej: etiqueta roja `"Impago"` o icono de alerta ⚠️)?
  * ¿Hay algún plazo o fecha límite antes de la competencia en el cual el sistema deba dar de baja automáticamente al atleta si continúa impago? ¿O la baja será 100% manual?
  * En el caso de botes múltiples (ej: K2, K4), si solo uno de los tripulantes está impago, ¿se debe marcar visualmente a todo el bote/embarcación como "Impago" o únicamente al individuo en falta?

### 2. Estructura de Costo del Evento
* **Pregunta**: Se va a agregar la propiedad **Costo de Inscripción** a la creación y edición del evento.
  * ¿Este costo es plano y único por cada inscripción de atleta, sin importar el tipo de bote o distancia?
  * ¿O se prevé que en el futuro el costo varíe según la categoría, la distancia o la embarcación (ej: botes individuales K1 vs botes grupales K4)?
  * ¿Pueden existir eventos gratuitos? (En cuyo caso, el sistema admitirá $0.00 como costo de inscripción por defecto).
  * ¿Los clubes que tienen planes de suscripción de alto nivel (como el Plan Oro) tienen algún tipo de bonificación o descuento sobre el costo de inscripción a eventos federativos?

---

## ⚙️ Otras Consultas Técnicas / Operativas

### 3. Registro de Pagos en el Sistema
* **Pregunta**: Actualmente, los pagos se pueden registrar de forma manual en la solapa de pagos.
  * Cuando se registre el pago de una inscripción, ¿el sistema debe validar automáticamente contra el **Costo de Inscripción** del Evento y marcar el saldo restante? ¿O se seguirá permitiendo ingresar cualquier monto de forma libre?
  * ¿Habrá soporte para pasarelas de pago online (ej: Mercado Pago, Stripe) en el futuro, o toda la conciliación continuará siendo mediante carga manual de transferencias y comprobantes?
