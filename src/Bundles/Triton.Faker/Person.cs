using System;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.MCART.Types.Extensions.RandomExtensions;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Representa a una persona generada aleatoriamente, además de que
/// contiene métodos estáticos que generan nuevas instancias
/// aleatorias de esta clase.
/// </summary>
public class Person
{
    private string? _userName;

    /// <summary>
    /// Obtiene el primer nombre de la persona.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Obtiene el apellido de la persona.
    /// </summary>
    public string Surname { get; }

    /// <summary>
    /// Obtiene el género biológico de la persona.
    /// </summary>
    public Gender Gender { get; }

    /// <summary>
    /// Obtiene la fecha de nacimiento de la persona.
    /// </summary>
    public DateTime Birth { get; }

    /// <summary>
    /// Obtiene un nombre de usuario generado para la persona.
    /// </summary>
    public string UserName => _userName ??= Internet.FakeUsername(this);

    /// <summary>
    /// Obtiene el nombre completo de la persona.
    /// </summary>
    public string Name => string.Join(' ', FirstName, Surname);

    /// <summary>
    /// Obtiene el nombre completo, como "Apellido, Nombre" de la persona.
    /// </summary>
    public string FullName => string.Join(", ", Surname, FirstName);

    /// <summary>
    /// Calcula y obtiene la edad de la persona el día de hoy.
    /// </summary>
    public double Age => (DateTime.Today - Birth).TotalDays / 365.25;

    private Person(string firstName, string surname, Gender gender, DateTime birth)
    {
        FirstName = Capitalize(firstName);
        Surname = Capitalize(surname);
        Gender = gender;
        Birth = birth;
    }

    /// <summary>
    /// Genera un adulto completamente aleatorio.
    /// </summary>
    /// <returns>Un adulto completamente aleatorio.</returns>
    public static Person Adult()
    {
        return Someone(18, 60);
    }

    /// <summary>
    /// Genera un niño completamente aleatorio.
    /// </summary>
    /// <returns>Un niño completamente aleatorio.</returns>
    public static Person Kid()
    {
        return Someone(5, 18);
    }

    /// <summary>
    /// Genera un bebé completamente aleatorio.
    /// </summary>
    /// <returns>Un bebé completamente aleatorio.</returns>
    public static Person Baby()
    {
        return Someone(0, 5);
    }

    /// <summary>
    /// Genera un adulto mayor completamente aleatorio.
    /// </summary>
    /// <returns>Un adulto mayor completamente aleatorio.</returns>
    public static Person Old()
    {
        return Someone(60, 110);
    }

    /// <summary>
    /// Genera una persona totalmente aleatoria.
    /// </summary>
    /// <param name="minAge">Edad mínima de la persona.</param>
    /// <param name="maxAge">Edad máxima de la persona.</param>
    /// <returns>
    /// Una persona totalmente aleatoria cuya edad se encuentra dentro del
    /// rango especificado de edades.
    /// </returns>
    public static Person Someone(int minAge, int maxAge)
    {
        var m = _rnd.CoinFlip();
        return new Person(
            (m ? StringTables.MaleNames : StringTables.FemaleNames).Pick(),
            StringTables.Surnames.Pick(),
            m ? Gender.Male : Gender.Female,
            FakeBirth(minAge, maxAge));
    }

    /// <summary>
    /// Genera una persona totalmente aleatoria.
    /// </summary>
    /// <returns>
    /// Una persona totalmente aleatoria.
    /// </returns>
    public static Person Someone()
    {
        return new Func<Person>[] { Baby, Kid, Adult, Old }.Pick().Invoke();
    }

    /// <summary>
    /// Genera una fecha aleatoria cuya edad en años se encuentra dentro
    /// del rango de edad especificado.
    /// </summary>
    /// <param name="minAge">Edad mínima a generar.</param>
    /// <param name="maxAge">Edad máxima a generar.</param>
    /// <returns>
    /// Una fecha aleatoria, la cual al ser aplicada a una persona, le
    /// proporciona de una edad que se encuentra dentro del rango de edades
    /// especificado.
    /// </returns>
    public static DateTime FakeBirth(int minAge, int maxAge)
    {
        var a = _rnd.NextDouble();
        return (DateTime.Today - TimeSpan.FromDays((a * (maxAge - minAge) + minAge) * 365.25)).Date;
    }
}
