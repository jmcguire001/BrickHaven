document.addEventListener("DOMContentLoaded", function () {
    var consent = localStorage.getItem('cookieConsent');
    if (!consent) {
        var el = document.getElementById('cookieConsent');
        el.style.display = 'block';
        el.style.animation = 'slideUp 1s forwards';
    }
});
function acceptCookies() {
    localStorage.setItem('cookieConsent', 'true');
    var el = document.getElementById('cookieConsent');
    el.style.animation = 'slideDown 1s forwards';
    setTimeout(function () { el.style.display = 'none'; }, 1000);
}