function guardarNoticia(event) {
    event.preventDefault();

    // Obtener los valores del formulario
    const titulo = document.getElementById('Titulo').value;
    const contenido = document.getElementById('Contenido').value;
    const fechaPublicacion = document.getElementById('FechaPublicacion').value;
    const imagen = document.getElementById('Imagen').files[0];

    // Validar que todos los campos estén completos
    if (!titulo || !contenido || !fechaPublicacion || !imagen) {
        mostrarMensaje('Por favor, completa todos los campos.', 'error');
        return;
    }

    // Convertir la imagen a base64
    const reader = new FileReader();
    reader.onload = function (event) {
        const imagenBase64 = event.target.result;

        // Crear el objeto noticia
        const noticia = {
            id: new Date().getTime(), // Usar timestamp como ID único
            titulo: titulo,
            contenido: contenido,
            imagenUrl: imagenBase64,
            fechaPublicacion: fechaPublicacion
        };

        // Guardar en localStorage
        let noticias = JSON.parse(localStorage.getItem('noticias')) || [];
        noticias.push(noticia);
        localStorage.setItem('noticias', JSON.stringify(noticias));

        // Mostrar mensaje de éxito
        mostrarMensaje('Noticia creada exitosamente!', 'success');

        // Redirigir a la página principal después de 2 segundos
        setTimeout(() => {
            window.location.href = '/Index';
        }, 2000);
    };
    reader.onerror = function () {
        mostrarMensaje('Error al cargar la imagen. Inténtalo de nuevo.', 'error');
    };
    reader.readAsDataURL(imagen);
}

// Función para cargar y mostrar noticias en la página principal
function cargarNoticias(contenedorId, mostrarBotonEliminar = false) {
    const noticias = JSON.parse(localStorage.getItem('noticias')) || [];
    const contenedorNoticias = document.getElementById(contenedorId);

    if (noticias.length === 0) {
        contenedorNoticias.innerHTML = '<p>No hay noticias disponibles.</p>';
    } else {
        contenedorNoticias.innerHTML = ''; // Limpiar antes de agregar nuevas

        noticias.forEach(noticia => {
            let noticiaHTML = document.createElement('div');
            noticiaHTML.classList.add('news-card'); // Usamos la clase news-card para el estilo

            // Crear el contenido de la noticia
            let contenidoNoticia = `
                <img src="${noticia.imagenUrl}" alt="${noticia.titulo}" class="noticia-imagen">
                <div class="noticia-contenido">
                    <h4 class="noticia-titulo">${noticia.titulo}</h4>
                    <p class="noticia-texto">${noticia.contenido}</p>
                    <p class="noticia-fecha"><small>Publicado el ${noticia.fechaPublicacion}</small></p>
            `;

            // Agregar el botón de eliminar solo si mostrarBotonEliminar es true
            if (mostrarBotonEliminar) {
                contenidoNoticia += `<button onclick="eliminarNoticia(${noticia.id})" class="btn-eliminar">Eliminar</button>`;
            }

            contenidoNoticia += `</div>`; // Cerrar el div de la noticia
            noticiaHTML.innerHTML = contenidoNoticia;
            contenedorNoticias.appendChild(noticiaHTML);
        });
    }
}

function eliminarNoticia(id) {
    let noticias = JSON.parse(localStorage.getItem('noticias')) || [];
    noticias = noticias.filter(noticia => noticia.id !== id);
    localStorage.setItem('noticias', JSON.stringify(noticias));
    cargarNoticias('noticias-perfil', true);
    mostrarMensaje('Noticia eliminada exitosamente.', 'success');
}

// Función para mostrar mensajes
function mostrarMensaje(mensaje, tipo) {
    const mensajeDiv = document.createElement('div');
    mensajeDiv.className = `alert alert-${tipo}`;
    mensajeDiv.textContent = mensaje;

    // Insertar el mensaje en el formulario o en la página
    const formulario = document.getElementById('noticiaForm');
    if (formulario) {
        formulario.prepend(mensajeDiv);
    } else {
        document.body.prepend(mensajeDiv);
    }

    // Eliminar el mensaje después de 3 segundos
    setTimeout(() => {
        mensajeDiv.remove();
    }, 5000);
}

// Asignar eventos al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    // Si estamos en la página de registro, asignar el evento al formulario
    if (document.getElementById('noticiaForm')) {
        document.getElementById('noticiaForm').addEventListener('submit', guardarNoticia);
    }

    // Si estamos en la página principal, cargar las noticias
    if (document.getElementById('noticias-dinamicas')) {
        cargarNoticias('noticias-dinamicas');;
    }
});
