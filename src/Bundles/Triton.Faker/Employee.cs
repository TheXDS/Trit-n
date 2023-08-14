using TheXDS.MCART.Types.Extensions;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Representa a un empleado generado aleatoriamente.
/// </summary>
public class Employee : Person
{
    /// <summary>
    /// Obtiene la dirección de correo electrónico del empleado.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Obtiene la posición del empleado dentro de la compañía.
    /// </summary>
    public string Position { get; }

    /// <summary>
    /// Genera un empleado en jefe de forma aleatoria.
    /// </summary>
    /// <param name="company">
    /// Compañía para la cual el empleado a generar labora.
    /// </param>
    /// <returns>
    /// Una nueva instancia de la clase <see cref="Employee"/>.
    /// </returns>
    public static Employee GetChief(Company company)
    {
        return ChiefFromPerson(Someone(35, 80), company);
    }

    /// <summary>
    /// Genera un empleado de forma aleatoria.
    /// </summary>
    /// <param name="company">
    /// Compañía para la cual el empleado a generar labora.
    /// </param>
    /// <returns>
    /// Una nueva instancia de la clase <see cref="Employee"/>.
    /// </returns>
    public static Employee Get(Company company)
    {
        return FromPerson(Adult(), company);
    }

    /// <summary>
    /// Convierte una instancia de la clase <see cref="Person"/> en una
    /// instancia de <see cref="Employee"/>, especificando la compañía para la
    /// cual el empleado generado trabaja.
    /// </summary>
    /// <param name="person">Persona a convertir en empleado.</param>
    /// <param name="company">
    /// Compañía para la cual el empleado generado trabaja.
    /// </param>
    /// <returns>
    /// Una nueva instancia de la clase <see cref="Employee"/>.
    /// </returns>
    public static Employee FromPerson(Person person, Company company)
    {
        return new(GetRandomPosition(), person, company);
    }

    /// <summary>
    /// Convierte una instancia de la clase <see cref="Person"/> en una
    /// instancia de <see cref="Employee"/> con rango de jefe, especificando la
    /// compañía para la cual el empleado generado trabaja.
    /// </summary>
    /// <param name="person">Persona a convertir en empleado.</param>
    /// <param name="company">
    /// Compañía para la cual el empleado generado trabaja.
    /// </param>
    /// <returns>
    /// Una nueva instancia de la clase <see cref="Employee"/>.
    /// </returns>
    public static Employee ChiefFromPerson(Person person, Company company)
    {
        return new(GetRandomChiefPosition(), person, company);
    }

    private Employee(string position, Person person, Company company)
        : base(person.FirstName, person.Surname, person.Gender, person.Birth)
    {
        Email = $"{person.UserName}@{company.DomainName}";
        Position = position;
        SetUserName(person.UserName);
    }

    private static string GetRandomChiefPosition()
    {
        return new[] {
            "CEO",
            "COO",
            "CTO",
            "CFO",
            "Director",
            "Manager",
            "VP Sales",
            "VP Marketing",
        }.Pick();
    }

    private static string GetRandomPosition()
    {
        return _rnd.CoinFlip() ? new[] {
            "Engineer",
            "Recepcionist",
            "Security officer",
            "Janitor",
            "Salesman",
            "HR Officer"
        }.Pick() : "Collaborator";
    }
}
