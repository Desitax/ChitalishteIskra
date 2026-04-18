using ChitalishteIskra.Models.Gallery;
using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            var albums = GetAlbums()
                .Select(album => new GalleryAlbumViewModel
                {
                    Slug = album.Slug,
                    Title = album.Title,
                    CoverImageUrl = album.CoverImageUrl,
                    PhotoCount = album.ImageUrls.Count
                })
                .ToList();

            return View(albums);
        }

        public IActionResult Details(string id)
        {
            var album = GetAlbums().FirstOrDefault(a => a.Slug == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(new GalleryDetailsViewModel
            {
                Slug = album.Slug,
                Title = album.Title,
                CoverImageUrl = album.CoverImageUrl,
                ImageUrls = album.ImageUrls
            });
        }

        private static List<GalleryDetailsViewModel> GetAlbums()
        {
            return new List<GalleryDetailsViewModel>
            {
                new GalleryDetailsViewModel
                {
                    Slug = "teatralen-mish-mash",
                    Title = "Театрален миш-маш",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370190/jda9iom7drw4nspilfg2.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370220/brgruyjftbsypdkekgy3.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370242/zoteye6gjayi2jcnijpk.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370271/soqesvbjtxzkslvwkyqz.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370307/irodhl9xiljbazwamaym.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776370393/q9jyn3jxxbkgu01p1ulc.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "liato-i-liubov",
                    Title = "Лято и любов",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372179/fjsyl1b0qce3erzbywow.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372239/qagjnc7xdmrb7ylze41j.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372179/fjsyl1b0qce3erzbywow.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372219/g4j9ca92edoplfg1xyzu.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372402/uxtiluui1hvnzxdrqpxr.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372435/iymqgjo6naih8wbxjcsj.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372482/ms0koek0tc5dphx5clrv.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372510/vuyn1wpn8br9oa0hpgeu.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372530/avfab4nvpeuacu3dzhq0.jpg",


                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372570/agqsyfdyqfln84auehul.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372587/gomt88hrfsmciodleuvn.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372609/exdqsklwnm6dlzjmwyso.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372642/snluca0zcbzro57cwym3.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372676/p4sx8sxf2mupgpvd6qpy.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372698/nn3g9k0zxh23pmi3thoy.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "iskrichka-2025",
                    Title = "35-то издание на конкурса за детска песен „Искричка“",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372897/v2nvdjw2iidegjef43ak.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372897/v2nvdjw2iidegjef43ak.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372928/zhomb71zdangvjzrv9zi.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372946/o9mvsibe2hkthnbpj9qf.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372965/mxbhku2fgck89xw1hfaj.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776372987/nccqudc239sppnttdcex.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "165-godini",
                    Title = "165 години дом на мечти и таланти",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776374423/wrdtvckd5vblbihn5nuv.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776374423/wrdtvckd5vblbihn5nuv.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776374904/vyzbeuygvbgt7y6ulhpu.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776374933/zbnqnu86rvprkb2xgra5.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776374691/bm1unf6er5bng67cpjua.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375156/abimvtuef1bcbtxhbzt2.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375194/dt3bwjfwba1vmdfmx88u.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375230/kqrzq2nqto3rvmixtsgx.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375267/iddq3wegdhufylhqgmtq.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375286/pkuoeprv82qq32cqd4iw.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375305/ruun8j8w7ppliqnlewy8.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375330/nqjdiubduzqjfrbrfmmo.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375349/zyntesbg0sjhmy4mb56h.jpg",


                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375375/wx4mlzxw7qvxdcgnbcvh.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375397/ax3qdxjfohwnslsobqpg.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375425/jodzivq3zzvg4on2lfa7.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375445/awt6i5vacdenmcajgydr.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375465/v8ftjj32y9kercc6k6xh.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776375488/minwpkabstjsgrgnuh5f.jpg",
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "folkloren-koncert",
                    Title = "Пролетен фолклорен концерт",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376371/j1gctqugkn9d348hgiua.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376371/j1gctqugkn9d348hgiua.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376421/ssox9qfyas7gbeappvxk.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376444/oispfiuz1yz7j2vkxcqc.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376469/hkvhayjxvchp4lrw8z1c.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376498/rbysveseuhwkbub2kmkg.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376523/muib94jombopqnrm9tfz.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376552/qkm3wbomqvn6yhvqo7p4.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376572/iufbso4avvzgxjwnmwyq.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "koleden-karnaval",
                    Title = "Коледен карнавал",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376991/dhnsa0ltc72tgkkyppnp.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776376991/dhnsa0ltc72tgkkyppnp.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377030/vglooajhs7ajjpklbqsu.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377047/fdp0erszt9uum6uvjota.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377081/gyiuoffroqabq3rpukfx.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377408/x67bfxe8yk6wbw9xfvjf.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377434/atwitg7eaaixlhp4oupa.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377461/waewzgzaqgic9gqi0ct3.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377481/f1q1omvywk4ail5r72te.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377502/lrmtq1upftquqrmtbbcu.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377536/bxnku1ljm9ot0jum1xbz.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377560/ppscuhrksoemrupmnorj.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377583/jwtltzk9vrphaate0chr.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377612/jheedhq5uoiae6joqm5z.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377629/p1lefln38juwo7742bav.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377646/wj1uwtne05gfniqufp9r.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377670/ttrldjqzxybtmyrjzirb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776377698/bopbv578lmy2vp2kgi8c.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "koledni-produkczii",
                    Title = "Коледни продукции от класовете по пиано, цигулка и китара",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378094/ervlnmadyfmhhoiodgz1.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378094/ervlnmadyfmhhoiodgz1.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378136/rcposnuwhfyb84m4gr4e.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378158/tmoydlqdinoupjm3evza.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378195/nshgoxjb4d8fxp9oijw7.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378223/e207gsqn5tuwtwugb1ch.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378242/h1vbxc4pdpzoxywopn2q.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378275/zswcpfe6sqyj7pd7tbao.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378293/t316v1tchm2xifnbeeme.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378311/omzlmwvfm0kg8pmmltft.jpg",


                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378337/ih9piq4ylpjrnxwywezc.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378366/gwyfmt7j7npo8phyzlff.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378387/j5wdplevlxkmizrtyat1.jpg",

                         "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378415/f3gzgm9nyqinh1lpzptv.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378435/y34wjo3rbok7hyhygvtb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378461/zo3mbkgaqfa7xguqp5ny.jpg",


                         "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378504/soa4gn4cpbtfkblx7hxb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378521/q3vbfq8iaskxekx0f8ru.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378540/idwfwygjmgz5fpnwbylr.jpg",


                         "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378557/a2gm3oieufj6po2k0nau.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378577/ywnmevf2dlhpx7wvqjnb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378595/ydh3uuavfcu2wn5qerr5.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776378630/z9kz9qupj69rrm7oe6sf.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "poklon-i-slava",
                    Title = "Поклон и слава вам, будители народни!",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379031/wgelujfxgrvmixcwfhqi.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379119/sstzporko5tsw4qhzxiy.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379148/at3pe4vczoatcuccb10b.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379315/x4klbko6ojdyjwoyy1e5.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379189/ggthv1pudb9rwb2rbmtb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379205/qa0lk362a7bfxnkgynt9.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379223/moh0l6lskgkefywixmcr.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776379240/w4gmridxq8o9rhhtnabp.jpg"
                    }
                },
                new GalleryDetailsViewModel
                {
                    Slug = "zeh-ta-radke-zeh-ta",
                    Title = "„Зех тъ, Радке, Зех тъ”",
                    CoverImageUrl = "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462602/xby4nilg3eufik2ukoqu.jpg",
                    ImageUrls = new List<string>
                    {
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776458933/pid4b0hemj4anecpmzah.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462576/zzzh6cxvstmivoi6zfrb.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462602/xby4nilg3eufik2ukoqu.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462623/ib6fqjmr6zxs6fututbh.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462637/grp59jlppibbljm2wd6i.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462662/kkwr0neijic7okmsfe4t.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462679/r6pe4pnzmcijidoknnf9.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462695/oxzxkkmbkuhvbuuad8yw.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462718/bqer5q48lggif8gutz76.jpg ",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462751/a7hjsfm39evsomtys8bn.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462780/ab6xohdyp0vn5rkprqoa.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462804/xwvqvxyqzc3w7jxzwqub.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462821/rre75irgqdsjoak6pify.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462837/adtf8jejlrqbsz1v0kv8.jpg ",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462855/vocgthbqjgdrkajcdsie.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462881/yh2c23hszokkcfgwkvby.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462900/q0yvkxpygnvrpuiw3nq6.jpg",

                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462934/wjb69ra1zevmycye4txi.jpg",
                        "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776462950/x2iioiwpg3kps7pvuiyr.jpg"
                    }
                },
            };
        }
    }
}