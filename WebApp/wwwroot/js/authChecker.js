function isUserLoggedIn() {
    const token = localStorage.getItem('JwtToken');
    return !!token;
}

function checkAuthPeriodically() {
    setInterval(() => {
        if (!isUserLoggedIn()) {
            console.log("Token missing. Redirecting...");
            window.location.href = '/';
        }
    }, 50000); // Check every 5 seconds
}

document.addEventListener('DOMContentLoaded', checkAuthPeriodically);
