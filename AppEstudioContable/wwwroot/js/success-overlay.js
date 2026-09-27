$(document).ready(function () {
    const overlay = document.getElementById('successOverlay');
    if (!overlay) return;

    // Ocultar después de 1.5 segundos
    setTimeout(function () {
        overlay.classList.add('fade-out');
        setTimeout(function () {
            overlay.remove();
        }, 300);
    }, 1500);
});