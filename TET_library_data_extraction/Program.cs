using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TET_dotnet;

namespace TET_library_data_extraction
{
    class Program
    {
        static void Main(string[] args)
        {
            string globaloptlist = @"licensefile {C:\Users\UPC_1\Documents\PDFlib\TET 5.1 64-bit\licensekeys.txt}";
            string basedocoptlist = "";
            string pageoptlist = "includebox={{0 45 600 790}} docstyle=forms imageanalysis={smallimages={disable}} layouthint={header=true footer=true} contentanalysis={dehyphenate=true} layouteffort=high granularity=line";
            string outfilebase = @"D:\_DFA_main\TetTest\Input\Section";
            var pdfPath = @"D:\_DFA_main\TetTest\Input\Section.pdf";
            var xmlPath = @"D:\_DFA_main\TetTest\Input\Section.tetml";
            TET tet = new TET();
            tet.set_option(globaloptlist);
            string docoptlist = "tetml={filename={" + xmlPath + "}} " + basedocoptlist;
            int doc = tet.open_document(pdfPath, docoptlist);

            int n_pages = (int)tet.pcos_get_number(doc, "length:pages");

            for (int pageno = 1; pageno <= n_pages; ++pageno)
            {
                //Get text data
                tet.process_page(doc, pageno, pageoptlist);
                //Get image data
                int page;
                int imagecount = 0;

                page = tet.open_page(doc, pageno, pageoptlist);

                if (page == -1)
                {
                    Console.WriteLine("Error {0} in {1}() on page {2}: {3}",
                        tet.get_errnum(), tet.get_apiname(), pageno, tet.get_errmsg());
                    continue; /* try next page */
                }

                while ((tet.get_image_info(page)) == 1)
                {
                    String imageoptlist;
                    int maskid;
                    imagecount++;
                    /* Write image data to file */
                    imageoptlist = "filename={" + outfilebase + "_p" + pageno + "_" + imagecount + "_I" + tet.imageid + "}";
                    if (tet.write_image_file(doc, tet.imageid, imageoptlist) == -1)
                    {
                        Console.WriteLine("\nError [" + tet.get_errnum() +
                        " in " + tet.get_apiname() + "(): " + tet.get_errmsg());
                        continue; /* try next image */
                    }

                    /* Check whether the image has a mask attached... */
                    maskid = (int)tet.pcos_get_number(doc,
                        "images[" + tet.imageid + "]/maskid");

                    /* and retrieve it if present */
                    if (maskid != -1)
                    {
                        imageoptlist = "filename={" + outfilebase + "_p" + pageno + "_" + imagecount + "_I" + tet.imageid + "mask_I" + maskid + "}";
                        if (tet.write_image_file(doc, tet.imageid, imageoptlist) == -1)
                        {
                            Console.WriteLine("\nError [" + tet.get_errnum() +
                            " in " + tet.get_apiname() +
                            "() for mask image: " + tet.get_errmsg());
                            continue; /* try next image */
                        }
                    }
                    if (tet.get_errnum() != 0)
                    {
                        Console.WriteLine("Error {0} in {1}() on page {2}: {3}",
                            tet.get_errnum(), tet.get_apiname(), pageno, tet.get_errmsg());
                    }
                }
                tet.close_page(page);
            }

            tet.process_page(doc, 0, "tetml={trailer}");
            tet.close_document(doc);


            //tet.get_tetml(@"C:\Users\Serjei\source\Andri_Diam\Diamond_project\TET_library_data_extraction\Section.pdf", pageoptlist);
        }
        static void report_image_info(TET tet, int doc, int imageid)
        {

            int width, height, bpc, cs, components, mergetype, stencilmask;
            String csname;


            width = (int)tet.pcos_get_number(doc,
                            "images[" + imageid + "]/Width");
            height = (int)tet.pcos_get_number(doc,
                            "images[" + imageid + "]/Height");
            bpc = (int)tet.pcos_get_number(doc,
                            "images[" + imageid + "]/bpc");
            cs = (int)tet.pcos_get_number(doc,
                            "images[" + imageid + "]/colorspaceid");
            components = (int)tet.pcos_get_number(doc,
                      "colorspaces[" + cs + "]/components");


            Console.Write("image {0}: {1}x{2} pixel, ", imageid, width, height);

            csname = tet.pcos_get_string(doc, "colorspaces[" + cs + "]/name");
            Console.Write(components + "x" + bpc + " bit " + csname);

            if (csname == "Indexed")
            {
                int basecs = 0;
                String basecsname;
                basecs = (int)tet.pcos_get_number(doc,
                    "colorspaces[" + cs + "]/baseid");
                basecsname = tet.pcos_get_string(doc,
                    "colorspaces[" + basecs + "]/name");
                Console.Write(" " + basecsname);
            }
            /* Check whether this image has been created by merging smaller images*/
            mergetype = (int)tet.pcos_get_number(doc,
                "images[" + imageid + "]/mergetype");
            if (mergetype == 1)
                Console.Write(", mergetype=artificial");

            stencilmask = (int)tet.pcos_get_number(doc,
                "images[" + imageid + "]/stencilmask");
            if (stencilmask == 1)
                Console.Write(", used as stencil mask");

            Console.WriteLine("");

        }
    }
}
