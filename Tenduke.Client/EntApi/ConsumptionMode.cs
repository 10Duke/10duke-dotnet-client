namespace Tenduke.Client.EntApi
{
    /// <summary>
    /// Consumption mode for license consumption calls.
    /// </summary>
    public enum ConsumptionMode
    {
        /// <summary>
        /// Short-term license consumption for online clients.
        /// </summary>
        Cache,

        /// <summary>
        /// Longer term license consumption for clients that may go offline.
        /// </summary>
        Checkout
    }
}
