using System;
using System.Reflection;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace NGWhitelist
{
    [ApiVersion(2, 1)]
    public class NGWhitelist : TerrariaPlugin
    {
        public override string Name { get { return "NGWhitelist"; } }
        public override string Author { get { return "Frontalvlad"; } }
        public override string Description { get { return "Plugin specifically for NGVille server. Adding to the whitelist by the player's nickname."; } }
        public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public static string NGPath = $"{TShock.SavePath}{Path.DirectorySeparatorChar}NGWhitelist{Path.DirectorySeparatorChar}";
        public static List<string> NGList = File.ReadAllLines($"{NGPath}ngwhitelist.txt").ToList();

        public NGWhitelist(Main game) : base(game) { }

        public override void Initialize() {
            Commands.ChatCommands.Add(new Command(WLEdit, new string[3] { "ngwhitelist", "ngwl", "wl" }));
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }
        protected override void Dispose(bool disposing) {
            ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
        }

        private void WLEdit(CommandArgs args) {
            if (args.Parameters.Count > 0) {
                if (args.Parameters[0].Equals("add", StringComparison.CurrentCultureIgnoreCase) && args.Player.HasPermission("ngwhitelist.add")) {
                    // Add to whitelist
                    if (args.Player.HasPermission("ngwhitelist.reload")) {
                        if (args.Parameters[1] != null) {
                            using (StreamWriter wl = new StreamWriter($"{NGPath}ngwhitelist.txt", true, Encoding.GetEncoding(1251))) {
                                wl.WriteLine($"{args.Parameters[1]}");
                            }
                            args.Player.SendInfoMessage($"[i:547][C/e3693f:NGWhiteList |] [C/808080:{args.Parameters[1]}] [C/ffffff:успешно добавлен в вайтлист]");
                        } else { args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/ffffff:Вы должны указать никнейм интересующего вас игрока."); }
                    } else { args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/ffffff:Извините, у вас нет прав на выполнение данной команды"); }
                } else if (args.Parameters[0].Equals("reload", StringComparison.CurrentCultureIgnoreCase)) {
                    // Update whitelist
                    if (args.Player.HasPermission("ngwhitelist.reload")) {
                        NGList = File.ReadAllLines($"{NGPath}ngwhitelist.txt").ToList();
                        args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/808080:Whitelist] [C/ffffff:успешно обновлен]");
                    } else { args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/ffffff:Извините, у вас нет прав на выполнение данной команды"); }
                } else if (args.Parameters[0].Equals("list", StringComparison.CurrentCultureIgnoreCase)) {
                    // Displaying players on the whitelist
                    if (args.Player.HasPermission("ngwhitelist.list")) {
                        args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/ffffff:Список игроков, допущенных на сервер:] [C/808080:{String.Join(", ", NGList)}]");
                    } else { args.Player.SendInfoMessage($"[i:547] [C/e3693f:NGWhiteList |] [C/ffffff:Извините, у вас нет прав на выполнение данной команды"); }
                } else { args.Player.SendInfoMessage($"[i:547] [C/ffffff:Команды:] [C/e3693f:NGWhiteList]\n[i:547] [C/ffffff:/ngwhitelist add] [C/808080:{args.Player.Name}] - [C/ffffff:Добавление в whitelist]\n[i:547] [C/ffffff:/ngwhitelist reload - Перезагрузка whitelist]\n[i:547] [C/ffffff:/ngwhitelist list - Список игроков в whitelist] "); }
            } else { args.Player.SendInfoMessage($"[i:547] [C/ffffff:Команды:] [C/e3693f:NGWhiteList]\n[i:547] [C/ffffff:/ngwhitelist add] [C/808080:{args.Player.Name}] - [C/ffffff:Добавление в whitelist]\n[i:547] [C/ffffff:/ngwhitelist reload - Перезагрузка whitelist]\n[i:547] [C/ffffff:/ngwhitelist list - Список игроков в whitelist] "); }
        }
        private void OnJoin(JoinEventArgs args) {
            if (!NGList.Contains(TShock.Players[args.Who].Name)) {
                TShock.Players[args.Who].Kick("[NGVille] Извините, вас нет в вайтлисте.");
            }
        }
    }
}