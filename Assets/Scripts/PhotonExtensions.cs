using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Photon.Pun;
using Photon.Realtime;

public static class PhotonExtensions {

    private static readonly Dictionary<string, string> SPECIAL_PLAYERS = new() {
        ["cf03abdb5d2ef1b6f0d30ae40303936f9ab22f387f8a1072e2849c8292470af1"] = "ipodtouch0218",
        ["d5ba21667a5da00967cc5ebd64c0d648e554fb671637adb3d22a688157d39bf6"] = "mindnomad",
        ["95962949aacdbb42a6123732dabe9c7200ded59d7eeb39c889067bafeebecc72"] = "MPS64",
        ["7e9c6f2eaf0ce11098c8a90fcd9d48b13017667e33d09d0cc5dfe924f3ead6c1"] = "Fawndue",
        ["9c06a1098b4e44e9d32d0fddd37a3a895eb31c8d81dfd895532ababf5f3bed79"] = "CalmDog", //editor
        ["dd0163ef2e320c020e63b874919415cc9ace605d7d238a18ae0688b985fbbf2e"] = "HyperCat", 
        ["73ed365a8c20253649d6b882629fc76a213e31f2d3d970def49de2a77360da2a"] = "BlueSwitchPalace", 
        ["81bb8dc26a12d7531c03eb1bb1810b53a6f3bc3798dcc05118aaa02bc4a8aa5e"] = "Yosh", 
        ["ce7c8371668803756928a575fa6ebd7ccde84026018e31d41b05c176882cd2ce"] = "MiiBumm", 
        ["cb989156b44e8885d203f12ecb75f7de0d6607835d2dc19b189fd660efe4526a"] = "FBEthePoS", 
        ["8f0fc456c10b08d93a21e60d4487709cc8415ef27c140a3d74c734f884013e1d"] = "vic", 
        ["fba5ff7e2b53b8b6135f498e5e6f3d3d34491702a4ec2b520aba7930156bb08c"] = "Cubby", 
    };

    public static bool IsMineOrLocal(this PhotonView view) {
        return !view || view.IsMine;
    }

    public static bool HasRainbowName(this Player player) {
        if (player == null || player.UserId == null)
            return false;

        byte[] bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(player.UserId));
        StringBuilder sb = new();
        foreach (byte b in bytes)
            sb.Append(b.ToString("X2"));

        string hash = sb.ToString().ToLower();
        return SPECIAL_PLAYERS.ContainsKey(hash) && player.NickName == SPECIAL_PLAYERS[hash];
    }

    //public static void RPCFunc(this PhotonView view, Delegate action, RpcTarget target, params object[] parameters) {
    //    view.RPC(nameof(action), target, parameters);
    //}
}