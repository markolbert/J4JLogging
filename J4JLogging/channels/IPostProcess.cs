namespace J4JSoftware.Logging
{
    // defines the functionality of types which are derived from TextChannel and are hence
    // capable of offering extended logging services. An example of this is the Twilio
    // channel.
    public interface IPostProcess
    {
        // Triggers the post-processing of whatever the current contents of the log channel
        // are. Typically this would be the last LogEvent because typically PostProcess() calls
        // Clear() -- which resets the channel's state -- when it completes.
        void PostProcess();

        // clears the log channel's state
        void Clear();

        // non-generic interface for configuring the post processor
        bool Initialize( object config );
    }

    public interface IPostProcess<in TSms> : IPostProcess
        where TSms : class
    {
        // Configures the post processor based on an instance of a configuration type
        bool Initialize( TSms config );
    }
}
