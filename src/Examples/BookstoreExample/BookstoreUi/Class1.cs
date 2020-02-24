using System;
using TheXDS.Triton.CrudGen;
using TheXDS.Triton.CrudGen.Base;
using TheXDS.Triton.Examples.BookstoreExample.Models;

namespace BookstoreUi
{
    public class BookCrudBuilder : CrudBuilder<Book>
    {
        protected override void Describe(ICrudDescriptionBuilder<Book> builder)
        {
            builder.Property(p => p.Name)
                .Label("Nombre")
                .Big();

            builder.Property(p => p.Edition)
                .Label("Edición")
                .Range(1, 10);
        }
    }
}
