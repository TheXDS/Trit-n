using System;
using TheXDS.Triton.CrudGen;
using TheXDS.Triton.CrudGen.Base;
using TheXDS.Triton.Examples.BookstoreExample.Models;

namespace BookstoreUi
{
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
