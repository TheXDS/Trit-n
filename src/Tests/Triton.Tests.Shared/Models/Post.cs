using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests.Models
{
    /// <summary>
    /// Describe un Post.
    /// </summary>
    public class Post : Model<long>
    {
        /// <summary>
        /// Título del <see cref="Post"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Fecha de creación del <see cref="Post"/>.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si el <see cref="Post"/> es
        /// público.
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Contenido del <see cref="Post"/>.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Referencia al autor del <see cref="Post"/>.
        /// </summary>
        public User Author { get; set; } = null!;

        /// <summary>
        /// Colección de comentarios sobre el <see cref="Post"/>.
        /// </summary>
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Post"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="title">Tìtulo del post.</param>
        /// <param name="content">Contenido del post.</param>
        /// <param name="author">Autor del post.</param>
        /// <param name="creationTime">Marca de tiempo de creación del post.</param>
        public Post(string title, string content, User author, DateTime creationTime) : this(title, content, creationTime)
        {
            Author = author;
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Post"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="title">Tìtulo del post.</param>
        /// <param name="content">Contenido del post.</param>
        /// <param name="author">Autor del post.</param>
        public Post(string title, string content, User author) : this(title, content, DateTime.Now)
        {
            Author = author;
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Post"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="title">Tìtulo del post.</param>
        /// <param name="content">Contenido del post.</param>
        /// <param name="creationTime">Marca de tiempo de creación del post.</param>
        public Post(string title, string content, DateTime creationTime)
        {
            Title = title;
            Content = content;
            CreationTime = creationTime;
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="Post"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="title">Tìtulo del post.</param>
        /// <param name="content">Contenido del post.</param>
        public Post(string title, string content) : this (title, content, DateTime.Now) { }
    }
}