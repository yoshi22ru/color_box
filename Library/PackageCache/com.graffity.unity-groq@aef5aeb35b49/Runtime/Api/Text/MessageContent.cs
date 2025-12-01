using Graffity.Groq.Common;

namespace Graffity.Groq.Text
{
    public class SystemMessage : BaseMessageContent
    {
        public SystemMessage(string content)
        {
            role = "system";
            this.content = content;
        }
    }
    
    public class UserMessage : BaseMessageContent
    {
        public UserMessage(string content)
        {
            role = "user";
            name = "test";
            this.content = content;
        }
    }
    
    public class AssistantMessage : BaseMessageContent
    {
        public AssistantMessage(string content)
        {
            role = "assistant";
            this.content = content;
        }
    }
    
    public class ToolMessage : BaseMessageContent
    {
        public ToolMessage(string content)
        {
            role = "tool";
            this.content = content;
        }
    }
}