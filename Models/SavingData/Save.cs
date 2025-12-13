using Bound.Managers;
using Bound.Models.Items;
using Bound.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models
{
    
    public class Save
    {
        #region

        private static List<int> _defaultLevels = new List<int>()
        {
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 4, 4, 4, 4, 3, 2
        };


        #endregion

        private static Player _player;
        private float _health;
        private float _mana;
        private float _stamina;
        private Game1 _game;

        //consider making these all private with a getter property
        public SaveManager Manager;
        public string Level;
        public string PlayerName;

        public int MoveSpeed;
        public int MaxDashes;
        public Vector2 Position;
        public int HotbarSlots;
        public int SkillSlots;

        private Inventory _inventory;
        private Dictionary<string, List<string>> _equippedItems = new Dictionary<string, List<string>>();
        private Dictionary<string, Attribute> _attributes;
        private List<Buff> _buffs = new List<Buff>();

        public Dictionary<string, List<string>> EquippedItems
        {
            get { return _equippedItems; }
        }

        public Dictionary<string, float> ItemStatBoosts
        {
            get
            {
                var stats = new Dictionary<string, float>();
                var equipped = new List<List<string>>() { _equippedItems["headgear"], _equippedItems["chestArmour"], _equippedItems["legArmour"], _equippedItems["footwear"] };
                Item i;
                foreach (var items in equipped)
                {
                    foreach (var item in items)
                    {
                        i = _game.Items[item];
                        foreach (var attr in i.Attributes)
                        {
                            if (stats.TryAdd(attr.Key, attr.Value.Value))
                                stats[attr.Key] += attr.Value.Value;
                        }
                    }
                }

                return stats;
            }
        }

        public Inventory Inventory
        {
            get { return _inventory; }
        }

        public List<Buff> Buffs
        {
            get { return _buffs; }
            set { _buffs = value; }
        }

        public Dictionary<string, Attribute> Attributes
        {
            get { return _attributes; }
            set { if (value != null) _attributes = value; }
        }

        public float Health
        {
            get { return _health; }
            set { _health = (value < 0) ? 0 : value; }
        }

        public float Mana
        {
            get { return _mana; }
            set { _mana = (value < 0) ? 0 : value; }
        }

        public float Stamina
        {
            get { return _stamina; }
            set { _stamina = (value < 0) ? 0 : value; }
        }

        public int MaxHealth
        {
            get
            {
                return GetMaxAttribute("Vigor", _defaultLevels);
            }
        }

        public int MaxMana
        {
            get { return GetMaxAttribute("Mind", _defaultLevels); }
        }

        public int MaxStamina
        {
            get { return GetMaxAttribute("Endurance", _defaultLevels); }
        }

        public void ResetAttrs()
        {
            _health = MaxHealth;
            _mana = MaxMana;
            _stamina = MaxStamina;
        }

        public override string ToString()
        {
            var str = EncryptKVP(Game1.Names.ID_PlayerName, PlayerName);
            str += EncryptKVP(Game1.Names.ID_CurrentLevel, Level);

            foreach (var item in Attributes)
                str += EncryptKVP(item.Key, item.Value.Value.ToString());

            str += EncryptKVP("Health", _health.ToString());
            str += EncryptKVP("Mana", _mana.ToString());
            str += EncryptKVP("Stamina", _stamina.ToString());

            if (Inventory.FlatInventory.Count > 0)
                str += EncryptKeyListPair("Inventory", Inventory.FormatForSave());
            if (EquippedItems.Count > 0)
                str += EncryptKeyListPair("EquippedItems", EquippedItems
                    .Select(x => String.Join(';', x.Value
                        .Select( y => $"{x.Key}: {y}").ToArray()))
                    .ToList());
            if (_buffs.Count > 0)
                str += EncryptKeyListPair("Buffs", _buffs.Select(x => x.Source + ", " + x.SecondsRemaining).ToList());

            str += EncryptKeyListPair("Position", new List<string>() { Position.X.ToString(), Position.Y.ToString() });

            return str;
        }


        private static void AddMissingKeys(SaveManager manager, Save save, List<string> attributeKeys)
        {
            foreach (var key in attributeKeys)
            {
                if (!save.Attributes.ContainsKey(key))
                    save.Attributes.Add(key, new Attribute(key, manager.DefaultAttributes[key]));
            }

            save.EquippedItems.Add("hotbar", Enumerable.Repeat("Default", 3).ToList());
        }

        public void SetEquippedItems(string input)
        {
            var items = input.Split(';').Select(x => x.Split(": "));
            EquippedItems.Clear();
            foreach (var item in items)
            {
                if (EquippedItems.ContainsKey(item[0]))
                    EquippedItems[item[0]].Add(item[1]);
                else
                    EquippedItems.Add(item[0], new List<string>() { item[1] });
            }
        }

        public string GetEquippedItem(string id, int index = 0)
        {
            if (EquippedItems.ContainsKey(id))
                return EquippedItems[id][index];
            else
                return "Default";
        }

        private int GetMaxAttribute(string attrKey, List<int> levelAdditions)
        {
            var value = 0;
            var limit = (int)Attributes[attrKey].Value;
            int overflow = (limit > levelAdditions.Count) ? limit - levelAdditions.Count : 0;
            for (int i = 0; i < limit - overflow; i++)
                value += levelAdditions[i];
            return value + (overflow * levelAdditions[^1]);
        }

        public void Update()
        {
            string name;
            foreach (var dict in EquippedItems)
                for (int i = 0; i < dict.Value.Count; i++)
                {
                    name = dict.Value[i];
                    if (name != "Default" && !Inventory.Contains(name))
                    {
                            if (dict.Key == "hotbar")
                            _player.RemoveItemFromHotbar(name);
                        dict.Value[i] = "Default";
                        i--;
                            
                    }
                }
        }

        public static void SetPlayer(Player player)
        {
            _player = player;
        }

        #region Encryption

        private static readonly Dictionary<char, string> _encryptionTable = new Dictionary<char, string>
        {
            { ' ', "D#hmo~#eN64nBD=378GEl&V1T~P,KRv(7+=(O6+(zfwns?f`!x[lKH4Gg,NAen}V"},
            { '!', "2xkfk%T*D{p#F50+JD4e%:8SJ>>1cU!JP{-e*c-/1t|XLMXKH/t.ZrEGK=](w9\"m"},
            { '\"', "[U>[ocX**R-AaaSwN7RQTJ$\'p7yikM<`q<NI&~fH{dhal0w~1Z{p4h_wP(/A`=~~"},
            { '#', "G^Nn)Ff`|6nx@9K6mJA<&,&n}Ob(f*LV_r B$NK][C9rXAiDsLK~(Pi5KZ*1W4Zk"},
            { '$', "<oYB&k!$xfxm;&9qpbu9c9tDbZTtp4O4plsmun<-\'Qy6u~<QK!>WG-Cx8# iY4o["},
            { '%', "Bgu&1o0WX2GF.0INK\'CMj]XK~~f?]+g_,?N#Nvu;_[jNmLE,TAD_{7\"UrU7vY879"},
            { '&', "vHvt>i>u2uWc!.r*Wn,Xb8\'&#k4/L*i~RB*[=Ep>7V@%k])1xgNw.1c$yVZLXfZ8"},
            { '\'', "|CTr}&1,7}*s:ct?*3f@(l<R(U^HHX?if/KR/IjEO%C#Hc~Nmr4bja|g:$(tD?Ew"},
            { '(', "|Y}1}Z`ZHD!GlFSJ89ChV|$*=Lb^/gm&<NC\')#tZ_A5Q(|j\"nMux$E*-^Wp>f%\"V"},
            { ')', "[^HyIrsgCg7V+1AIN8RL+\",~.0^-U4P8fR?d*e@VPi#C#!(N\"Ga9+i[N,v>p`}<b"},
            { '*', "6ycuw$r`\'M\"q<>[(\"bes]t[SiH-PWH3ZZLKdndI-[Y.Br*.Sp+u!1ii)U[HG\"> ?"},
            { '+', "pB6akN2 /a&&l$53c]M-Mj+!Q53rC~56X<_dkC*uNL|+=`*=wJyUQD+iauz@X29+"},
            { ',', "ZgRsr&9nV^_RZJuL5$!:%WIvDH+-gOE |x`br$6j0wAmHp$2+:tWVMNEF!!\'0!MM"},
            { '-', "AQO }Lb*0@<t27KXik6A3Hsr%2^TiDU4be~L;`\"Q3`OKw)p3%8lZITxCH{7MB0Fa"},
            { '.', "=C x/@kk5)FDyS?=HLwm,JL?@H]%m]%g3B/Ra-i}n_[w+G$%Ue2M(uF+;oVu>ts6"},
            { '/', "llPUgM~!OWx_LtY#4U\"gd[-9SV_l[ahG+-\'xua+E\'EvEpJhb:meB+85h~i\"|G_qJ"},
            { '0', "!xX,Q=&OaYZ\'$Z? Z+d,qAsbwecXj\"_lw`WqR[dWk)+\">9FbpdQB*0/Ea0O<|#<f"},
            { '1', "|-)gI-h|X;\",Q:z|{MWU*s]j3`^h9ls*]D7\';eaAE{=3J6X7nj=i)J{{c(Sa4u<c"},
            { '2', "MuuEN%,=$=/4W_?x`H|<M `tg?*.&Do2l[6Ut;)8jn.$493$mkndR~@Xs}i;h/jp"},
            { '3', "Vx*->kYoX9O-[68._\'Rd6Pnd%#s&6 m8J|@CK.zg5^Hxjq-a89mczmI=9({~6QOF"},
            { '4', "4$zmQ66`4g{c9$i{d{+QOlq_hoYOa~FUCqTzR) yYiGiIM-ac9JW sn{OPh$JX;V"},
            { '5', "/L7i?bUn?dq-8+a\'N]Ur%M7iWr.g9CYzqc&omfsS\"jTjkD:{A^$&{.Z:n5a`j:Bi"},
            { '6', "A^; (Gt5nsdW1f|AE!`=n]VJnj9e1Md4%wGdHCKXkg$ gbJ_Nxk^a[;np$k.YQ_g"},
            { '7', "Gw2 s}G7lc_jvVJFB8>YC4R?o5H3\'\'yzPz2GR0@,*SzLAtyP?+oqU1\">#W|b/>>N"},
            { '8', "f %]w/kzW66_PfB[{ ~DnrIg\'jHPbk)`CH\"6%z^Vj2W#4^v.^+lMR6O)M#i.UWls"},
            { '9', "Ca0lnZ}Zz=8Ry9{Ju?hv`I+h9^`dv*-uH4&T7YalyXplfZQ+j\"AOwX.Rd(t*z3$m"},
            { ':', "<HzQ+Qd/[+o0Cg.wK,h\"b}m.5Wce]1PDZ:-%CJ8q4547SJXbqPEO\"*#LY~TS,lQU"},
            { ';', "yk(yO`n{vo%+CUdz\"+sjEZDU3\'+od2p(S{$qx|Zd~;LOF$in~  AH3E1[Y;JD`t0"},
            { '<', "0{gO,~^IWEQfc}e|*bpq,6pI#;)pkp6S |y6@YZ5-RC*v?s5[()}z7aZ)ecrM4&a"},
            { '=', "EM,ZonW1G0E}7O-(GRkmNK>wY\"|adAF@V5gBqrmk3x-K1t}t9Wgp^S6G>jW9nnVB"},
            { '>', "6RR->%`s.2Cq95\'UN-?mB ?[:Ck{q$%X4:BR!Elza^yN:$nn;neY-t9?%f^eE\"Z-"},
            { '?', "p@p~S>K%,#`wm`oK\"TM9{TLChC}[#G&/YGEIgcWmIq^5.;\'.LvnM|XoHUi\'#?.AC"},
            { '@', "Cp*1YCb4U(890pWfAn*opa4 v?mx2P .HC,3AU`POZ%f*%~?e\",cG}GLSSU{TJQo"},
            { 'A', "jgi-zxU@f0lI/%24_`KxW\'PN7v(tI*5+f46jXk~7X&y9RAFSZrU7yz!k1nYn*}uC"},
            { 'B', "op${z:xZ$UAyO5Ew5oU3;qFf+s~B;#O{^htKBW+#C\'WpUlh\"\"|EZ/4bA[5O`85P-"},
            { 'C', "<DY)J6*lrJ/z|M0<|RR\'XDG}@7=TSu9MJS2BM;Aq/2^N6G9.4\'4/{!^/-_KGQ$ m"},
            { 'D', "Vj<?9\"Km nFq2i2a?KDS0:*]<_)MO)+Y(F5T4MFbq>`](D\"4jnnTk9s?yVTsxVLH"},
            { 'E', "%(bU%_9G*xOJE >32FEuUPd&2<5cctVT^p|fhD=}[>BU8)L[ML6Yf\'V7/j3c65$T"},
            { 'F', "/3T:]3^3,L)Z-@\'bw_t49+Y?ujrQ%I)VPlq{1YL3yWZ;!e/t=HSXEnoKT&f}BgyF"},
            { 'G', "NXIct>a&>+CP>U3J]8w6pCe@wV2(!|7pD6a,1LH&XcaI3H<7WMu@YBrYr,\"/&>[O"},
            { 'H', "x!/49Nu)A7*|>znMSUt0jh\'9V$a1B\'@hs\"5\' jFVbuVPhR8)z|/+\'jLw 8\"V}]Y<"},
            { 'I', "C&_Gw+*;O`x_v-dQ8U:4$]d%1CC~\'l4% |3~J^EY<0#tc6vjy-\'\"^w2@\'C1D \'<l"},
            { 'J', "~,\"e3v}+Ov$g<#.1{BvJdN2L=;Q2lF4-7+yFDK}g3~cYJzsV&F9.y3pHQZG$5(G2"},
            { 'K', ".s?KTd>w/t9U4^_ [<}#$Uy]9bxnmnL R-UKj)Z\'Ev3q030|bJq~AoKBuN#tT_@;"},
            { 'L', "lx7 M@ua8>Q1TV@R2Q{m9!%XqF\'.(-Xwg0uS)ad+,Y[ZLKKo=8)HTczv3])+q*NH"},
            { 'M', "q|e@:&6cn5hnDCOPHOd^p/((yDq{.#8[Qov>JC+<8#M(Z:\"=<PbTy\'0]}j{GLwrh"},
            { 'N', "eV(mt8le,4^P~W]]|6Cw}^!)/,g%|i(sPOz_An>QAdG4wN?< (>45>Gi}\"-QjTD\'"},
            { 'O', "EHb[<1B;Wro8[EP}?C&87F]{i{!LkKKJg2j_^{!=+d7VQCx#FhJ_P6nPJQIo&I\'b"},
            { 'P', "H+T<S~$o7>Q^i:N+|?q,YE}u4eCdrLp-Zp0|6+v0$VZF?_^F<\'wL nB7x[dg8Y\'C"},
            { 'Q', "{}b`bb84B}B?x!,6DIG0;cbY7V5V^rh;z`t#5e+/cmB>Y3gSru2=|;(Qtv 4Co_z"},
            { 'R', "fqo-c%]2td9tDkSqS)9O/Rq(GI*Qpd/lB8uGg{1T<kT8\"+30r+c$kJj-Z!6EYa9;"},
            { 'S', "{E;}.R{F>IjB>$V`1K4s^e,x3~qJ16lrfb<!@f>N4$=r& Y=.v7&#;!bXp/.0$;V"},
            { 'T', "fO\'D<7owJvAS7-.M*}`=\"!1u{<6mBLn.0;N/`KNy..to}(X10fN9SurH$]U1:HUw"},
            { 'U', "bSgs\"]R*deY8[Gu>3?vPv0Q<Z7Q<f;$x:p/dyOR/qvLlKtXs5BI^]quzR,y w8q\'"},
            { 'V', "jha4\"FGBjQgnx;CG==j{u.e7S?D8t;&X#I|p4s+3x{K &>SO6uskKB,eybX*-;g,"},
            { 'W', ",D&/Bx-prfX{d{Qv\"|s6/T|QPRby!1p*~/7=B5O=dqr*ysd&U)[>)rBVXNE.(Ue0"},
            { 'X', "2,(.2wyb(~XpqoY-LaB8*q&NV_p4XCH@X>Jt$BT?ZW;xQKiZWz.9Urm^`nCDLhfr"},
            { 'Y', "jO^C-ly6s($Qhuo;\"Ja<>NO=|zr)htKFxvsA|)bvmH!(n$h=kD\"%f)BB$2Db.~[`"},
            { 'Z', "H8oboH@m:JOW|j( ZbB\"AA&u4W^_8sCO+BYX^~*_\"9;k\"hjeR;Pzw$-_Te8jqYy\'"},
            { '[', "qU+HOdY@7D<9*2FtI $)z}h?|*.WY5fxlj*}Cp5:&1j<&tV.Gk\"f,6>3c^z#ACae"},
            { ']', "L^NDj_`)zuFGqY(B4t*9`m4ZV?YbrP),zsVBTOp-jORA._|4xOFE7jGS9RyMF0qG"},
            { '^', "U%\'braE#ZeE0L9=sRsgsd?Rb`;vLvK?rV0AS2>=QP`#2@Q8V({xV }`m{81`VUT]"},
            { '_', "@09!E0(~k3-[>C9dVTDrHv&vZwhbW7SE<\"& Yg5X*cOuhe^Wrbyr1>!zdI q/]UH"},
            { '`', "g)fvp:kEE!zDPdmm`b0K~T85|h=0L?s]Udir,Y]gfZtH21\"2>n2#5<G<,Xpf-@i*"},
            { 'a', "8bul4%|BCy+mB&(O1cp\"JWBI|BeT`FG$G(=*9?la#G#2E(=-`&VNt:A<$:o FtC&"},
            { 'b', "#{7-c!yM_\"z0;bxTzWq5+,sLmQuI*8XNQmEqu67Sfi!m-KqtH&zLVnhIqH4 qF\' "},
            { 'c', "S.d| `j5Bbdo :?E,gZ(salg5uUlk}I4QLi5H41NPi\"`VS$[BqsS\"3E&,ao }|ut"},
            { 'd', "S8S\'wjK64y5;]v6!p&jEG/Gkia*5kzE+,(m]\"EIU3bTsfip/=,q$~nv>p qjpyk["},
            { 'e', ";?o=4}.1#Fx:+nx}Y6tv]4hF3$C(4q{#:#pdgCEM]O8R|N-]yh {OA}h5Xw({qQ\'"},
            { 'f', "`X*IwWxZ8I4UpJ=m??2-k?I*z)\'i&2)#GmE_F3t*ikh*apny>OzHT2?bEKVZ:kS7"},
            { 'g', "$5 %oQbxRrkA-Bl14s@aV3(,]+,lw@P!&afbm![Q1Xk_,p4q)#4a-pFkSH6Q^)1c"},
            { 'h', "fk\'lzWj5hd{1)W[4(s:v/1:bz{5uEcgq5%33=Ht7\'}?]~%9vu1_;IH1{\"8~K,$-X"},
            { 'i', "C@`(R({aH9\"G9KkTHNe0~_uv.V|<`\"Gj7/@ZVz9pB2\'Hk??1CsP1t$z,/M*Rer*E"},
            { 'j', "ulj7f19C5*M{|S2KR,xjMep/4n&1=qdxQT!V~-@STr W%A:5OxU/?jUG^3NG^bp7"},
            { 'k', "$VY\"#`Eq{qTO4*P^F!]uqT4\'W%k*3GJlS4unjL<-\'OKs:]E1,Q@9S,icpW&V{lYb"},
            { 'l', "QDBs<;jEKu.>D,_AlmXAt`s)=tB-ueF)5P?SRdtXS_{O!5u?p=()v\'4fZ{F\"vDqR"},
            { 'm', "07O{*0k}=(KIGO]cDw]$)i:|_SPyNjenTkX[k4Ld@Vv<\"6k2GE\'o{Qy/<t)i=)kd"},
            { 'n', "F%\"sUjz1?Z$qf)W].!Q<8\'9*W#I3G_/0r4~,WFUV*z,u[CSvDV\"i$wH`f.~t[NnC"},
            { 'o', "z}33-ohW!e|wMWWEXUg12BeD0f;Z%C$J .m0O5)E\"|Xea6dEY*S>%aFDplZ<wH;g"},
            { 'p', "J@KZCwIJfG7)YCAU8V#xZ$=ipC*uqBZ&x2s@LR)UE6)fc]C04K(s&R|na6}Ku[Ih"},
            { 'q', "6BI{^\"me&+EK\"t~Uy$S+_YKQO7&HXu;8l@GaI77GwXMigJ8VQf>1{=LhkrJN1ro4"},
            { 'r', "[ qx@N0YN9\'4\'2qXkjwiZ:bcOdaX|j/r}5t]&;^Ggry=\'\"PmQ8kbgI bve}1eMyr"},
            { 's', "rx_n14Vte<-7JG\"bdG98vlsNT=_}uoaX]NQL[#,MUhVz6>c}oC3}^c(V41R1(2Fj"},
            { 't', "`0H\'\'tg-,!|v<5={hN7wQ<@rLdh#R_dl0F[Mu9g8,7Iz!A})65i{^49-lgqy,ny/"},
            { 'u', ")|#v[2T6f%mad&ateNPtb<XA@JxG/`nDv`8l6n/z=~*5zmkwkhGG,)L<sa9|IVWD"},
            { 'v', "|\"mvI@5pU}VtA)j-?Jf+YD+Q=h[\"%ch#a,^jN.!mF4Y^[$t<P|k!&KoB~% 3?6H\""},
            { 'w', ">J&yd=}\"U8z#ejWFfcjk>YCP@umFb( ]rd*@O)wsoZekSV;~+>gVp~]ue\"Qsc:G,"},
            { 'x', "^WUX])|{PuL]Si9-Tk?gHg<&{x$ewpoiy!zM-,\'Hwz&qw1LhsOGCUf,)HcEG.P]Z"},
            { 'y', "o;[s.DQ?W)TvQ63j=|p+#qLlH{$O7-}<iM@&9bPt\'n^wq5w%=+\' C*IQH2{}}7gk"},
            { 'z', "-rJ6IPB0S)?};\'#j=cbb5tt>y3[r,x)j+lB)sH%D!agC>}I4gR=ua)?sA4pOq=_*"},
            { '{', "NJd?D*1!R2[C7?t7:C|l|oM{e&8~z)8Ff|)8J6SvhKpOf5vpo|UiD1vqLY^FiRIe"},
            { '|', ":fsZ3Ak-L@0D+I&oS,Eeh,X;%h~5}: +]HKc*-^C ^7EQ{#|) __ )[.~f#[tFnG"},
            { '}', "%yp~o\"Ta[+i99fr`uTLqaZ{{-9Qf,`TfI@ |Gc;f(Vb(2bc[{F4@,:#Y0Fx#UTQJ"},
            { '~', "ffeA9m6I:d\"T|zH9, aeGH{%H/aARgN{~)W+G8iMhQl73#zgDmX-f8vk(/bh@{rJ"},
        };
        private static readonly string SEPERATOR = "Zxy,&)F! suKCjxa0umo/sJ&#CN!nOwvW~OB/oN+@[?|Xr0~0=xbInhmrtNKdIDM";
        private static readonly Dictionary<string, char> _decryptionTable = _encryptionTable.ToDictionary(x => x.Value, x => x.Key);
        private static readonly int _encryptedCharLength = 64; //maybe decrease this in the future as save files become larger

        private static string Encrypt(string s) => String.Join(String.Empty, s.Select(c => _encryptionTable[c]));
        private static string EncryptKVP(string id, string value) => Encrypt(id) + SEPERATOR + Encrypt(value) + "\n";
        private static string EncryptKeyListPair(string id, List<string> list) => Encrypt(id) + SEPERATOR + String.Join(SEPERATOR, list.Select(x => Encrypt(x))) + "\n";//when this is decrypted, a semicolon will be between each item

        private static string Decrypt(string s) => String.Join(String.Empty, s.Chunk(_encryptedCharLength).ToList().Select(x => _decryptionTable[new string(x)]));
        private static (string Key, string Value) DecryptLine(string line)
        {
            var kvp = line.Split(SEPERATOR);
            if (kvp.Length < 2)
                return (Decrypt(kvp[0]), null);
            if (kvp.Length > 2)
                return (Decrypt(kvp[0]), String.Join(';', kvp.Skip(1).Select(x => Decrypt(x))));

            return (Decrypt(kvp[0]), Decrypt(kvp[1]));
        }

        #endregion

        //returns a new save
        #region New Saves

        private Save(Game1 game, SaveManager manager)
        {
            _game = game;
            _inventory = new Inventory(game, game.Player);
            Manager = manager;
            _attributes = new Dictionary<string, Attribute>();
        }

        public static Save NewSave(SaveManager manager, Game1 game)
        {
            var save = new Save(game, manager)
            {
                Level = "newgame",
                PlayerName = "_",
            };

            AddMissingKeys(manager, save, manager.DefaultAttributes.Keys.ToList());

            return save;
        }

        public static Save ImportSave(SaveManager manager, List<string> lines, Game1 game)
        {
            try
            {
                var save = new Save(game, manager)
                {
                    Manager = manager,
                };

                var AttributeKeys = manager.DefaultAttributes.Keys.ToList();

                foreach (var line in lines)
                {
                    var kvp = DecryptLine(line);

                    if (AttributeKeys.Contains(kvp.Key))
                        save.Attributes.Add(kvp.Key, new Attribute(kvp.Key, int.Parse(kvp.Value)));
                    else
                    {
                        switch (kvp.Key)
                        {
                            case "Inventory":
                                var inventory = kvp.Value.Split(';').ToList();
                                foreach (var item in inventory)
                                    save.Inventory.Import(item);
                                break;
                            case "Position":
                                var pos = kvp.Value.Split(';');
                                save.Position = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                                break;
                            case "Level":
                                save.Level = kvp.Value; break;
                            case "PlayerName":
                                save.PlayerName = kvp.Value; break;
                            case "EquippedItems":
                                save.SetEquippedItems(kvp.Value); break;
                            case "Health":
                                save.Health = save.FloatTryParse(kvp.Value); break;
                            case "Mana":
                                save.Mana = save.FloatTryParse(kvp.Value); break;
                            case "Stamina":
                                save.Stamina = save.FloatTryParse(kvp.Value); break;
                            case "Buffs":
                                save.Buffs = kvp.Value.Split(';').Select(x => x.Split(", ")).Select(x => new Buff(game, x[0], float.Parse(x[1]))).ToList(); break;
                        }
                    }
                }

                return save;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Message: " + e.Message);
                return null;
            }
        }


        private float FloatTryParse(string input)
        {
            float output;
            if (float.TryParse(input, out output))
                return output;
            return 0f;
        }
        #endregion

    }
}
