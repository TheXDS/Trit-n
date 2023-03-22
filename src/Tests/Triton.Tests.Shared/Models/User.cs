using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests.Models
{
    /// <summary>
    /// Describe a un usuario.
    /// </summary>
    public class User : Model<string>
    {
        /// <summary>
        /// Nombre a mostrar.
        /// </summary>
        public string PublicName { get; set; }

        /// <summary>
        /// Fecha de registro.
        /// </summary>
        public DateTime Joined { get; set; }

        /// <summary>
        /// Colección de todos los post creados por el usuario.
        /// </summary>
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        /// <summary>
        /// Colección de todos los comentarios creados por el usuario.
        /// </summary>
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="User"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="id">Id de la entidad.</param>
        /// <param name="publicName">Nombre a mostrar.</param>
        public User(string id, string publicName) : this(id, publicName, DateTime.Now)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia del modelo <see cref="User"/>,
        /// especificando el valor de los campos que no pueden ser
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="id">Id de la entidad.</param>
        /// <param name="publicName">Nombre a mostrar.</param>
        /// <param name="joined">Fecha en la que se ha unido el usuario.</param>
        public User(string id, string publicName, DateTime joined) : base(id)
        {
            PublicName = publicName;
        }
    }
}