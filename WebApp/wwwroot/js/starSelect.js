<script>
    document.getElementById("clearRating").addEventListener("click", function () {
        const radios = document.querySelectorAll('input[name="Rating"]');
        radios.forEach(r => r.checked = false);
    });
</script>
