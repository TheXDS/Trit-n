using System;
using TheXDS.Triton.Annotations;
using TheXDS.Triton.Component;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.ViewModel;

namespace TheXDS.Triton.Models
{
    public class GeneralSecurityDescriptor: ModelBase<Guid>
    {
        public MethodCategory Granted { get; set; }
        public MethodCategory Revoked { get; set; }
    }

    public class GeneralSecurityDescriptorViewModel : ViewModel<GeneralSecurityDescriptor, Guid>
    {

    }
}