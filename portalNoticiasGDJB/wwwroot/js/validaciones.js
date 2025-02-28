function validarLoginForm(event) {
    let usuario = document.getElementById('usuario').value;
    let contrasena = document.getElementById('contrasena').value;

    if (usuario.trim() === '' || contrasena.trim() === '') {
        alert('Por favor, completa todos los campos.');
        event.preventDefault();
        return false;
    }

    // Validar que el correo tenga un formato correcto
    let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(usuario)) {
        alert('Por favor, ingresa un correo electrónico válido.');
        event.preventDefault();
        return false;
    }

    // Validar longitud de la contraseña
    if (contrasena.length < 6) {
        alert('La contraseña debe tener al menos 6 caracteres.');
        event.preventDefault();
        return false;
    }

    localStorage.setItem('usuario', usuario);

    let email = usuario;
    asignarRol(email);
    mostrarOpcionesPorRol();

    return true;
}

function validarRegistro(event) {
    let nombre = document.getElementById('nombre').value;
    let email = document.getElementById('email').value;
    let password = document.getElementById('password').value;
    let confirmPassword = document.getElementById('confirm-password').value;

    // Validar nombre (no vacío)
    if (nombre.trim() === '') {
        alert('Por favor, ingresa tu nombre.');
        event.preventDefault();
        return false;
    }

    // Validar correo electrónico con regex
    let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
        alert('Por favor, ingresa un correo electrónico válido.');
        event.preventDefault();
        return false;
    }

    // Validar que la contraseña tenga al menos 6 caracteres
    if (password.length < 6) {
        alert('La contraseña debe tener al menos 6 caracteres.');
        event.preventDefault();
        return false;
    }

    // Validar que las contraseñas coincidan
    if (password !== confirmPassword) {
        alert('Las contraseñas no coinciden.');
        event.preventDefault();
        return false;
    }

    return true;
}

// Validación para el formulario de registro de noticia
function validarNoticia(event) {
    let titulo = document.getElementById('Titulo').value;
    let imagen = document.getElementById('Imagen').value;
    let contenido = document.getElementById('Contenido').value;
    let fechaPublicacion = document.getElementById('FechaPublicacion').value;

    if (titulo.trim() === '') {
        alert('Por favor, ingresa un título para la noticia.');
        event.preventDefault();
        return false;
    }

    if (contenido.trim() === '') {
        alert('Por favor, ingresa el contenido de la noticia.');
        event.preventDefault();
        return false;
    }

    if (imagen.trim() === '') {
        alert('Por favor, selecciona una imagen para la noticia.');
        event.preventDefault();
        return false;
    }

    if (!fechaPublicacion) {
        alert('Por favor, ingresa una fecha de publicación válida.');
        event.preventDefault();
        return false;
    }

    return true;
}


