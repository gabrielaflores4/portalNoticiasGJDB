// Lista de noticias identificadas por un ID único
const noticiasIds = ["noticia1", "noticia2", "noticia3", "noticia4", "noticia5", "noticia6"];

// Cargar todas las reacciones y comentarios al iniciar la página
document.addEventListener("DOMContentLoaded", function () {
    noticiasIds.forEach(newsId => {
        loadNewsReactions(newsId);
        loadComments(newsId);
    });
});

// 🟢 FUNCIONES PARA REACCIONES 🟢

// Cargar reacciones desde localStorage
function loadNewsReactions(newsId) {
    let newsReactions = JSON.parse(localStorage.getItem("newsReactions")) || {};

    if (!newsReactions[newsId]) {
        newsReactions[newsId] = { likes: 0, dislikes: 0 };
    }

    let reactionsSection = document.getElementById(`reactions-${newsId}`);
    if (reactionsSection) {
        reactionsSection.innerHTML = `
            <button onclick="reactToNews('${newsId}', 'like')">👍 Like (${newsReactions[newsId].likes})</button>
            <button onclick="reactToNews('${newsId}', 'dislike')">👎 Dislike (${newsReactions[newsId].dislikes})</button>
        `;
    }
}

// Manejo de reacciones
function reactToNews(newsId, type) {
    let newsReactions = JSON.parse(localStorage.getItem("newsReactions")) || {};

    if (!newsReactions[newsId]) {
        newsReactions[newsId] = { likes: 0, dislikes: 0 };
    }

    if (type === "like") {
        newsReactions[newsId].likes++;
    } else if (type === "dislike") {
        newsReactions[newsId].dislikes++;
    }

    localStorage.setItem("newsReactions", JSON.stringify(newsReactions));
    loadNewsReactions(newsId); // Recargar para mostrar cambios
}

// 🟡 FUNCIONES PARA COMENTARIOS 🟡

// Cargar comentarios desde localStorage
function loadComments(newsId) {
    let comments = JSON.parse(localStorage.getItem("comments")) || {};

    if (!comments[newsId]) {
        comments[newsId] = [];
    }

    let commentsSection = document.getElementById(`comments-section-${newsId}`);
    if (commentsSection) {
        commentsSection.innerHTML = "";

        if (comments[newsId].length > 0) {
            comments[newsId].forEach(comment => {
                let commentElement = document.createElement("div");
                commentElement.classList.add("comment");
                commentElement.innerHTML = `<strong>${comment.username}:</strong> ${comment.text}`;
                commentsSection.appendChild(commentElement);
            });
        } else {
            commentsSection.innerHTML = "<p>No hay comentarios aún. Sé el primero en comentar.</p>";
        }
    }
}

// Agregar un comentario
function addComment(newsId) {
    let usernameInput = document.getElementById(`username-${newsId}`).value.trim();
    let commentText = document.getElementById(`comment-text-${newsId}`).value.trim();

    if (usernameInput === "" || commentText === "") {
        alert("Por favor, completa todos los campos antes de comentar.");
        return;
    }

    let comments = JSON.parse(localStorage.getItem("comments")) || {};

    if (!comments[newsId]) {
        comments[newsId] = [];
    }

    comments[newsId].push({ username: usernameInput, text: commentText });

    localStorage.setItem("comments", JSON.stringify(comments));

    // Limpiar los inputs
    document.getElementById(`username-${newsId}`).value = "";
    document.getElementById(`comment-text-${newsId}`).value = "";

    loadComments(newsId); // Recargar para mostrar el nuevo comentario
}
