function mostrarOpcionesPorRol() {
    let rol = localStorage.getItem('rol');
    let adminActions = document.getElementById('admin-actions');
    let userActions = document.getElementById('user-actions');
    let adminContent = document.getElementById('admin-content');
    let userContent = document.getElementById('user-content');

    if (adminActions && userActions && adminContent && userContent) {
        if (rol === 'admin') {
            adminActions.style.display = 'block';
            adminContent.style.display = 'block';
            userActions.style.display = 'none';
            userContent.style.display = 'none';
            cargarNoticias('noticias-perfil', true);
        } else if (rol === 'usuario') {
            userActions.style.display = 'block';
            userContent.style.display = 'block';
            adminActions.style.display = 'none';
            adminContent.style.display = 'none';
        } else {
            redirigirAInicio();
        }
    } else {
        console.log('Error: Algunos elementos no fueron encontrados.');
    }
}

function redirigirAInicio() {
    window.location.href = '/';
}

function asignarRol(email) {
    if (email.endsWith('@noticias.com')) {
        localStorage.setItem('rol', 'admin');
    } else {
        localStorage.setItem('rol', 'usuario');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    let rol = localStorage.getItem('rol');
    if (rol) {
        mostrarOpcionesPorRol();
    }

    let usuario = localStorage.getItem('usuario');
    if (usuario) {
        let datosUsuario = document.querySelector('.datos-usuario');
        if (datosUsuario) {
            let usuarioSinDominio = usuario.split('@')[0];
            datosUsuario.querySelector('h2').textContent = usuarioSinDominio;
            datosUsuario.querySelector('p').textContent = `Email: ${usuario}`;
        }
    }

    let cerrarSesionBtn = document.getElementById('cerrar-sesion');
    if (cerrarSesionBtn) {
        cerrarSesionBtn.addEventListener('click', cerrarSesion);
    }
});

function cerrarSesion() {
    localStorage.removeItem('usuario');
    localStorage.removeItem('rol');
    window.location.href = '/index';
}
