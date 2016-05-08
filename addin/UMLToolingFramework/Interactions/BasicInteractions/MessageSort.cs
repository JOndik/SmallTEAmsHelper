using System.Collections.Generic;
using UML = TSF.UmlToolingFramework.UML;


namespace TSF.UmlToolingFramework.UML.Interactions.BasicInteractions {

    //This is an enumerated type that identifies the type of communication action that was used to generate the message.
    public enum MessageSort : int {

        //The message was generated by a synchronous call to an operation.
        synchCall,

        //The message was generated by an asynchronous call to an operation (i.e., a CallAction with
        //“isSynchronous = false”).
		asynchCall,

        //The message was generated by an asynchronous send action.
		asynchSignal,

        //The message designating the creation of another lifeline object.
        createMessage,

        //The message designating the termination of another lifeline.
        deleteMessage,

        //The message is a reply message to an operation call.
        reply
	}
}