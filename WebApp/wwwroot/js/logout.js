document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm");
    const logoutButton = document.getElementById("logoutButton");
    const logoutModal = document.getElementById("logoutModal");
    const confirmLogout = document.getElementById("confirmLogout");
    const cancelLogout = document.getElementById("cancelLogout");

    if (!logoutForm || !logoutButton || !logoutModal) return;

    logoutButton.addEventListener("click", function (e) {
        e.preventDefault();
        logoutModal.classList.remove("hidden");
    });

    confirmLogout.addEventListener("click", function () {
        localStorage.clear();
        localStorage.setItem("logout", Date.now());
        logoutForm.submit();
    });

    cancelLogout.addEventListener("click", function () {
        logoutModal.classList.add("hidden");
    });
});
