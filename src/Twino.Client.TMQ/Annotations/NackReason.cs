namespace Twino.Client.TMQ.Annotations
{
    /// <summary>
    /// Negative Acknowledge reasons
    /// </summary>
    public enum NackReason
    {
        /// <summary>
        /// Reason none
        /// </summary>
        None,

        /// <summary>
        /// Reason error
        /// </summary>
        Error,

        /// <summary>
        /// Reason is class name of the exception
        /// </summary>
        ExceptionType,

        /// <summary>
        /// Reason is messge of the exception
        /// </summary>
        ExceptionMessage
    }
}