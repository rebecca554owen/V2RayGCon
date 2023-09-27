﻿using Newtonsoft.Json;

namespace V2RayGCon.Models.Datas
{
    public class Vmess
    {
        public string ps,
            add,
            port,
            id,
            aid,
            net,
            type,
            host,
            tls,
            v,
            path,
            sni;

        public Vmess()
        {
            v = string.Empty; // v1:"" v2:"2"
            ps = string.Empty; // alias
            add = string.Empty; // ip,hostname
            port = string.Empty; // port
            id = string.Empty; // user id
            aid = string.Empty;
            net = string.Empty; // ws,tcp,kcp

            tls = string.Empty; // streamSettings->security
            sni = string.Empty; // tlsSettings.serverName

            type = string.Empty; // kcp->header
            host = string.Empty; // v1: ws->path v2: ws->host h2->["host1","host2"]
            path = string.Empty; // v1: "" v2: ws->path h2->path
        }

        public Vmess(SharelinkMetadata vc)
            : this()
        {
            v = "2";
            ps = vc.name;
            add = vc.host;
            port = vc.port.ToString();
            id = vc.auth1;
            net = vc.streamType;
            tls = vc.tlsType == "tls" ? "tls" : "none";
            sni = vc.tlsServName;

            switch (net)
            {
                case "grpc":
                    path = vc.streamParam2;
                    break;
                case "quic":
                    type = vc.streamParam1;
                    host = vc.streamParam2;
                    path = vc.streamParam3;
                    break;
                case "tcp":
                    type = vc.streamParam1;
                    path = vc.streamParam2;
                    host = vc.streamParam3;
                    break;
                case "kcp":
                    type = vc.streamParam1;
                    path = vc.streamParam2;
                    break;
                case "ws":
                    path = vc.streamParam1;
                    host = vc.streamParam2;
                    break;
                case "h2":
                    path = vc.streamParam1;
                    host = vc.streamParam2;
                    break;
                default:
                    // unsupported stream type
                    break;
            }
        }

        public bool Equals(Vmess t)
        {
            if (
                t == null
                || !t.sni.Equals(this.sni)
                || !t.v.Equals(this.v)
                || !t.ps.Equals(this.ps)
                || !t.add.Equals(this.add)
                || !t.port.Equals(this.port)
                || !t.id.Equals(this.id)
                || !t.aid.Equals(this.aid)
                || !t.net.Equals(this.net)
                || !t.type.Equals(this.type)
                || !t.host.Equals(this.host)
                || !t.path.Equals(this.path)
                || !t.tls.Equals(this.tls)
            )
            {
                return false;
            }
            return true;
        }

        public string ToVmessLink()
        {
            var vmess = this;
            if (vmess == null)
            {
                return null;
            }

            string content = JsonConvert.SerializeObject(vmess);
            return Misc.Utils.AddLinkPrefix(
                VgcApis.Misc.Utils.Base64EncodeString(content),
                VgcApis.Models.Datas.Enums.LinkTypes.vmess
            );
        }
    }
}
