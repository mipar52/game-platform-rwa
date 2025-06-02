function confirmLogout() {
    if (confirm("Are you sure you want to log out?")) {
        const form = document.createElement("form");
        form.method = "POST";
        form.action = "/Login/Logout";

        // Add anti-forgery token if it exists
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) {
            const input = document.createElement("input");
            input.type = "hidden";
            input.name = "__RequestVerificationToken";
            input.value = token.value;
            form.appendChild(input);
        }

        document.body.appendChild(form);
        form.submit();
    }
}
