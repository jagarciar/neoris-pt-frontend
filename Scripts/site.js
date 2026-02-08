// JavaScript para funcionalidad del sitio
(function() {
    'use strict';

    // Log cuando la página carga
    console.log('Neoris PT Frontend cargado correctamente');

    // Función para formatear precios
    window.formatCurrency = function(amount) {
        return new Intl.NumberFormat('es-MX', {
            style: 'currency',
            currency: 'MXN'
        }).format(amount);
    };

    // Función para formatear fechas
    window.formatDate = function(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('es-MX');
    };

    // Animación suave para los enlaces
    document.addEventListener('DOMContentLoaded', function() {
        // Agregar animación a las tarjetas
        const cards = document.querySelectorAll('.card');
        cards.forEach(function(card, index) {
            setTimeout(function() {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                
                setTimeout(function() {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 100);
            }, index * 100);
        });
    });

})();
