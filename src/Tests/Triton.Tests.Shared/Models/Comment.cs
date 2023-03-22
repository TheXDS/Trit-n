using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests.Models
{
    /// <summary>
    /// Describe un comentario.
    /// </summary>
    public class Comment : Model<long>
    {
        /// <summary>
        /// Autor del comentario.
        /// </summary>
        public User Author { get; set; } = null!;

        /// <summary>
        /// <see cref="Post"/> en el cual se ha dejado este comentario.
        /// </summary>
        public Post Parent { get; set; } = null!;

        /// <summary>
        /// Fecha en la que se ha creado este comentario.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Contenido del comentario.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Comment"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="author">Autor del comentario.</param>
        /// <param name="parent">
        /// Post en el cual se ha realizado el comentario.
        /// </param>
        /// <param name="content">Contenido del comentario.</param>
        /// <param name="timestamp">Fecha de creación del comentario.</param>
        public Comment(User author, Post parent, string content, DateTime timestamp) : this(content, timestamp)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Comment"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="author">Autor del comentario.</param>
        /// <param name="parent">
        /// Post en el cual se ha realizado el comentario.
        /// </param>
        /// <param name="content">Contenido del comentario.</param>
        public Comment(User author, Post parent, string content) : this(author, parent, content, DateTime.Now)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Comment"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="content">Contenido del comentario.</param>
        public Comment(string content) : this(content, DateTime.Now)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Comment"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="content">Contenido del comentario.</param>
        /// <param name="timestamp">Fecha de creación del comentario.</param>
        public Comment(string content, DateTime timestamp)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            Content = content;
            Timestamp = timestamp;
        }
    }
}