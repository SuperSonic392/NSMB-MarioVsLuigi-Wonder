using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Photon.Pun;
using Photon.Realtime;

public static class PhotonExtensions {

    private static readonly Dictionary<string, string> SPECIAL_PLAYERS = new() {
        ["cf03abdb5d2ef1b6f0d30ae40303936f9ab22f387f8a1072e2849c8292470af1"] = "ipodtouch0218", //please join my mod so i can update this :)
        ["d5ba21667a5da00967cc5ebd64c0d648e554fb671637adb3d22a688157d39bf6"] = "mindnomad",
        ["95962949aacdbb42a6123732dabe9c7200ded59d7eeb39c889067bafeebecc72"] = "MPS64",
        //["7e9c6f2eaf0ce11098c8a90fcd9d48b13017667e33d09d0cc5dfe924f3ead6c1"] = "Fawndue",
        ["9c06a1098b4e44e9d32d0fddd37a3a895eb31c8d81dfd895532ababf5f3bed79"] = "CalmDog", //editor
        ["dd0163ef2e320c020e63b874919415cc9ace605d7d238a18ae0688b985fbbf2e"] = "HyperCat", 
        ["73ed365a8c20253649d6b882629fc76a213e31f2d3d970def49de2a77360da2a"] = "BlueSwitchPalace", //frien :3
        ["81bb8dc26a12d7531c03eb1bb1810b53a6f3bc3798dcc05118aaa02bc4a8aa5e"] = "Yosh", //frien :3
        ["ce7c8371668803756928a575fa6ebd7ccde84026018e31d41b05c176882cd2ce"] = "MiiBumm", //massive thanks
        ["cb989156b44e8885d203f12ecb75f7de0d6607835d2dc19b189fd660efe4526a"] = "FBEthePoS", 
        ["8f0fc456c10b08d93a21e60d4487709cc8415ef27c140a3d74c734f884013e1d"] = "vic", 
        ["fba5ff7e2b53b8b6135f498e5e6f3d3d34491702a4ec2b520aba7930156bb08c"] = "Cubby", 
        ["26aa0abe8729eb88cb172d21bcce44796dbb8951091b1458bb7c44f859a5cc9b"] = "FrostyCake", //massive thanks
        ["2f944ff55765d5dd1e60c0f83877ba617508b36f6564c916da36eda6980e1766"] = "Pez", //<- it's him 
        ["2337a92af0af2ad044b82e7508ec7da435dfaed9d846ab373468c38d6dd4877b"] = "PezUno", //thank you Dom
        ["f67501c5280803674ff30caebd3b56918ac6ca686cc6751f54b2933f41eee4f6"] = "PezDos", //thank you HugoAndFriends
        ["447546d58b424ca40c809b632eb1eaf81a8e27b1757dfbe0300f16b3bc97f2ab"] = "PezTres", //thank you YawnY062. edit: frien :3
        ["a5031a1b00605f4f7b096b3b74c22aedbfc5670a65b8bbe68a307b9a89c94bd1"] = "PezCuatro", //thank you Jumbledsmile267
        ["5758c3c9c93ea5ce4fb7d7d18b2706b618186a098b18674e0138270d76da6a10"] = "PezCinco", //thank you Huluigi, fellow Peak player.
        ["ebf9bc7091cb18364ddd405df8d9487e90c55bc7384d227ac512b7bc5cc24704"] = "PezSeis", //thank you Ultimatum
        ["dbbb27dc53eadb4cc36519e4345212e7449d601365a3f4cacf090d5ac67f45d2"] = "PezSiete", //thank you Zub
        ["5ea80e99021b946d60e67270192593e8d722a9c677768ffb008b311da6c81261"] = "PezOcho", //thank you FredBearMlG
        ["9186991bea8fcc328330f24276f1f8ebaa9753a5aecef5b537d62a2cbcc6e36b"] = "PezNueve", //thank you SprDeleto
        ["86fb8d86dbb4629b62fc056fa55bf816006874f86d914c52e9d587acfb225f43"] = "Kazik", //frien :3
        ["22c6b4c9b4e625d29206305af805bfdd08656afff46f914867dd35e1b431f439"] = "Grape", 
        ["fb457661def32025bd32df3c1c3ba6bdecb8c4c1230931da4f911b533a05126b"] = "KirbyKid",
        ["e1ccfcb9f51c9baf0a4668386d72421750368b76fced0b1bf0946588f3dad64d"] = "SHITMAN",
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