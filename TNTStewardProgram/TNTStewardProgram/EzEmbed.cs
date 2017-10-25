using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace TNTStewardProgram
{
    class EzEmbed : ModuleBase
    {

        EmbedBuilder embed;
        EmbedFooterBuilder footer;
        public string[] colores = 
            {
            "azul",
            "verde",
            "turquesa",
            "rojo",
            "lila",
            "amarillo",
            "blanco",
            "gris",
            "negro",
            "azul_claro",
            "verde_claro",
            "turquesa_claro",
            "rojo_claro",
            "lila_claro",
            "amarillo_claro",
            "blanco_brillante"
            };
              

        private void SetEmbedColor(string color)
        {
            embed = new EmbedBuilder();

            int r = 0;
            if (color.ToLower() == "random")
            {
                Random rnd = new Random();
                r = rnd.Next(0, 16) + 1;
            }

            if (color.ToLower() == "azul" || r == 1) { embed.WithColor(new Color(0, 0, 255)); }
            else if (color.ToLower() == "verde" || r == 2) { embed.WithColor(new Color(0, 255, 0)); }
            else if (color.ToLower() == "turquesa" || r == 3) { embed.WithColor(new Color(0, 255, 255)); }
            else if (color.ToLower() == "rojo" || r == 4) { embed.WithColor(new Color(255, 0, 0)); }
            else if (color.ToLower() == "lila" || r == 5) { embed.WithColor(new Color(127, 0, 255)); }
            else if (color.ToLower() == "amarillo" || r == 6) { embed.WithColor(new Color(255, 255, 0)); }
            else if (color.ToLower() == "blanco" || r == 7) { embed.WithColor(new Color(223, 223, 223)); }
            else if (color.ToLower() == "gris" || r == 8) { embed.WithColor(new Color(160, 160, 160)); }
            else if (color.ToLower() == "azul_claro" || r == 9) { embed.WithColor(new Color(153, 204, 255)); }
            else if (color.ToLower() == "negro" || r == 10) { embed.WithColor(new Color(0, 0, 0)); }
            else if (color.ToLower() == "verde_claro" || r == 11) { embed.WithColor(new Color(153, 255, 153)); }
            else if (color.ToLower() == "turquesa_claro" || r == 12) { embed.WithColor(new Color(153, 255, 255)); }
            else if (color.ToLower() == "rojo_claro" || r == 13) { embed.WithColor(new Color(255, 153, 153)); }
            else if (color.ToLower() == "lila_claro" || r == 14) { embed.WithColor(new Color(204, 153, 255)); }
            else if (color.ToLower() == "amarillo_claro" || r == 15) { embed.WithColor(new Color(255, 255, 153)); }
            else if (color.ToLower() == "blanco_brillante" || r == 16) { embed.WithColor(new Color(255, 255, 255)); }
            else { embed.WithColor(new Color(255, 255, 255)); }
        }

        public void CreateFooterEmbed(string color, string title = null, string description = null, string thumbnailurl = null, string footer_text = null, string footer_thumbnail = null)
        {
            SetEmbedColor(color);

            if (title != null && title != "none") { embed.WithTitle(title); }
            if (description != null && description != "none") { embed.WithDescription(description); }
            if (thumbnailurl != null && thumbnailurl != "none") { embed.WithThumbnailUrl(thumbnailurl); }

            footer = new EmbedFooterBuilder();

            if (footer_text != null && footer_text != "none")
            {
                embed.WithFooter(footer
                     .WithText(footer_text)
                );
            }
            if (footer_thumbnail != null && footer_thumbnail != "none")
            {
                embed.WithFooter(footer
                     .WithIconUrl(footer_thumbnail)
                );
            }
        }

        public void CreateBasicEmbed(string color, string title = null, string description = null, string thumbnailurl = null)
        {
            SetEmbedColor(color);

            if (title != null && title != "none") { embed.WithTitle(title); }
            if (description != null && description != "none") { embed.WithDescription(description); }
            if (thumbnailurl != null && thumbnailurl != "none") { embed.WithThumbnailUrl(thumbnailurl); }
        }

        public async Task SendEmbed(ICommandContext context)
        {
            await context.Channel.SendMessageAsync("", false, embed);
        }
        public async Task SendEmbed(IUser user)
        {
            await user.SendMessageAsync("", false, embed);
        }
    }
}
