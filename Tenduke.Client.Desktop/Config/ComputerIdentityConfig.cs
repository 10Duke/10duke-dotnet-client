using System;
using Tenduke.Client.Desktop.Util;

namespace Tenduke.Client.Desktop.Config
{
    /// <summary>
    /// Configuration used for determining how this system is identified against the 10Duke Entitlement service.
    /// </summary>
    [Serializable]
    public class ComputerIdentityConfig
    {
        /// <summary>
        /// Configured computer identifier. If not <c>null</c>, this value will be used and
        /// all other configuration is ignored.
        /// </summary>
        public string ComputerId { get; set; }

        /// <summary>
        /// <para>Components to use for computing the computer identifier.</para>
        /// <para>If value of <see cref="ComputerId"/> is not <c>null</c>, this property is ignored.</para>.
        /// </summary>
        public ComputerIdentity.ComputerIdentifier[] ComputeBy { get; set; }

        /// <summary>
        /// <para>Additional application-specified part to use as an additional component with <see cref="ComputeBy"/>
        /// when computing the computer identifier.</para>
        /// <para>If value of <see cref="ComputerId"/> is not <c>null</c>, this property is ignored.</para>.
        /// </summary>
        public string AdditionalIdentifier { get; set; }

        /// <summary>
        /// Application specific salt to use for computing the computer identifier hash value.
        /// </summary>
        public string Salt { get; set; }
    }
}
