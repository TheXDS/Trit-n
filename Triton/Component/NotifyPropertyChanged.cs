﻿/*
NotifyPropertyChanged.cs

This file is part of TheXDS! Tritón Framework

Author(s):
     César Andrés Morgan <xds_xps_ivx@hotmail.com>

Copyright © 2018 César Andrés Morgan

This program is free software: you can redistribute it and/or modify it under
the terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with
this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheXDS.Triton.Annotations;

namespace TheXDS.Triton.Component
{
    /// <inheritdoc />
    /// <summary>
    ///     Implementa la funcionalidad básica de la interfaz
    ///     <see cref="T:System.ComponentModel.INotifyPropertyChanged" />.
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <inheritdoc />
        /// <summary>
        ///     Ocurre cuando el valor de una propiedad cambia.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Genera el evento <see cref="PropertyChanged"/> notificando del
        ///     cambio de la propiedad con el nombre especificado.
        /// </summary>
        /// <param name="propertyName">
        ///     Opcional. Nombre de la propiedad que ha cambiado su valor. si se
        ///     omite, se establece en el nombre de la propiedad que ha realizado
        ///     la llamada.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch
            {
#if DEBUG
                System.Diagnostics.Debug.Print(Resources.Strings.Err_Invoking_OnPropertyChanged, propertyName);
#endif
                /* silenciar excepción */
            }
        }
    }
}