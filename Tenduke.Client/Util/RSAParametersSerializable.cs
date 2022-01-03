using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// <see cref="RSAParameters"/> is not serializable in .NET Core. This class works around
    /// this in order to allow serializing and deserializing RSA public keys.
    /// </summary>
    [Serializable]
    public class RSAParametersSerializable : ISerializable
    {
        private RSAParameters _rsaParams;

        public RSAParameters RSAParameters => _rsaParams;

        public RSAParametersSerializable()
        {
        }

        public RSAParametersSerializable(RSAParameters rsaParameters)
        {
            _rsaParams = rsaParameters;
        }

        public RSAParametersSerializable(SerializationInfo info, StreamingContext context)
        {
            _rsaParams = new RSAParameters()
            {
                D = (byte[])info.GetValue(nameof(RSAParameters.D), typeof(byte[])),
                DP = (byte[])info.GetValue(nameof(RSAParameters.DP), typeof(byte[])),
                DQ = (byte[])info.GetValue(nameof(RSAParameters.DQ), typeof(byte[])),
                Exponent = (byte[])info.GetValue(nameof(RSAParameters.Exponent), typeof(byte[])),
                InverseQ = (byte[])info.GetValue(nameof(RSAParameters.InverseQ), typeof(byte[])),
                Modulus = (byte[])info.GetValue(nameof(RSAParameters.Modulus), typeof(byte[])),
                P = (byte[])info.GetValue(nameof(RSAParameters.P), typeof(byte[])),
                Q = (byte[])info.GetValue(nameof(RSAParameters.Q), typeof(byte[])),
            };
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(RSAParameters.D), _rsaParams.D);
            info.AddValue(nameof(RSAParameters.DP), _rsaParams.DP);
            info.AddValue(nameof(RSAParameters.DQ), _rsaParams.DQ);
            info.AddValue(nameof(RSAParameters.Exponent), _rsaParams.Exponent);
            info.AddValue(nameof(RSAParameters.InverseQ), _rsaParams.InverseQ);
            info.AddValue(nameof(RSAParameters.Modulus), _rsaParams.Modulus);
            info.AddValue(nameof(RSAParameters.P), _rsaParams.P);
            info.AddValue(nameof(RSAParameters.Q), _rsaParams.Q);
        }

        //
        // Summary:
        //     Represents the D parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] D { get => _rsaParams.D; set => _rsaParams.D = value; }

        //
        // Summary:
        //     Represents the DP parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] DP { get => _rsaParams.DP; set => _rsaParams.DP = value; }

        //
        // Summary:
        //     Represents the DQ parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] DQ { get => _rsaParams.DQ; set => _rsaParams.DQ = value; }

        //
        // Summary:
        //     Represents the Exponent parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] Exponent { get => _rsaParams.Exponent; set => _rsaParams.Exponent = value; }

        //
        // Summary:
        //     Represents the InverseQ parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] InverseQ { get => _rsaParams.InverseQ; set => _rsaParams.InverseQ = value; }

        //
        // Summary:
        //     Represents the Modulus parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] Modulus { get => _rsaParams.Modulus; set => _rsaParams.Modulus = value; }

        //
        // Summary:
        //     Represents the P parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] P { get => _rsaParams.P; set => _rsaParams.P = value; }

        //
        // Summary:
        //     Represents the Q parameter for the System.Security.Cryptography.RSA algorithm.
        public byte[] Q { get => _rsaParams.Q; set => _rsaParams.Q = value; }
    }
}
