﻿using System;
using System.Threading.Tasks;

namespace Discord.Commands.Attributes.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequirePermission : PreconditionAttribute
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        public RequirePermission(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }
        public RequirePermission(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }
        
        public override Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance)
        {
            var guildUser = context.Author as IGuildUser;

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return Task.FromResult(PreconditionResult.FromError("Command must be used in a guild channel"));                
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError($"Command requires guild permission {GuildPermission.Value}"));
            }

            if (ChannelPermission.HasValue)
            {
                var guildChannel = context.Channel as IGuildChannel;

                ChannelPermissions perms;
                if (guildChannel != null)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(guildChannel);

                if (!perms.Has(ChannelPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError($"Command requires channel permission {ChannelPermission.Value}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}