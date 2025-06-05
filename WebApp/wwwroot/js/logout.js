document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm");
    const logoutButton = document.getElementById("logoutButton");

    if (!logoutForm || !logoutButton) return;

    logoutButton.addEventListener("click", function (e) {
        e.preventDefault();

        const confirmed = confirm("Are you sure you want to log out?");
        if (confirmed) {
            logoutForm.submit();
            localStorage.clear();
            localStorage.setItem("logout", Date.now());
        }
    });
});
