<script>
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.app-alert--dismissible').forEach(function (el) {
            setTimeout(function () {
                el.classList.add('fade-out');
                setTimeout(function () { el.remove(); }, 300);
            }, 5000);
        });
    });
</script>