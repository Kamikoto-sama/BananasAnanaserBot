# Welcome to the MultiplatformBotFramework wiki!

 # Multiplatform Bot Framework

Do you write bots for various platforms, such as Telegram or Discord?

Has it ever happened that you need a bot for several platforms at once?

Or the situation that you need to switch to another API and now again you need to rewrite everything?

## Main features



## Short Description

## Main classes

- **Session**
 
	This class is the basis for interacting with the user when performing more complex tasks.
    Allows you to create an interaction component that will be responsible for a specific part of the bot's logic.

### Abstractions
- **IExternalContentProvider**

	Special type for providing external content for session.
	It is initiated once with a session and doesn't change in Session reset method.
	Can use for getting text from external database, for getting pictures and ect.