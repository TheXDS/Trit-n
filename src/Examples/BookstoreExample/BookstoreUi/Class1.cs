using System;
using TheXDS.Triton.CrudGen;
using TheXDS.Triton.CrudGen.Base;
using TheXDS.Triton.Examples.BookstoreExample.Models;

namespace BookstoreUi
{
    public class AuthorCrudBuilder : CrudBuilder<Author>
    {
        protected override void Describe(ICrudDescriptionBuilder<Author> editor, ICrudDescriptionBuilder<Author> viewer)
        {
            editor.Property(p => p.Name)
                .Label("Nombre");

            editor.Property(p => p.Bio)
                .Big()
                .Label("Biografía del autor");

            editor.Property(p => p.Picture)
                .Label("Fotografía del autor")
                .PicturePath();

            editor.Property(p => p.Books)
                .Label("Libros del autor")
                .CreateAndSelect();
        }
    }

    public class BookCrudBuilder : CrudBuilder<Book>
    {
        protected override void Describe(ICrudDescriptionBuilder<Book> editor, ICrudDescriptionBuilder<Book> viewer)
        {
            editor.Property(p => p.Name)
                .Label("Nombre")
                .Big();

            editor.Property(p => p.Edition)
                .Label("Edición")
                .Range(1, 10);
        }
    }
}
