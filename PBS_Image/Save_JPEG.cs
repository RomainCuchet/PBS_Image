using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class Save_JPEG
    {
        #region Header
        public struct Header
        {
            public byte[] SOI;
            public byte[] APP0;
            public byte[] DQT;
            public byte[] SOF0;
            public byte[] DHT;
            public byte[] SOS;
        }

        public void set_SOI(Header h)
        {
            //intialization
            h.SOI = new byte[2];

            //Marker
            h.SOI[0] = 0xff;
            h.SOI[1] = 0xd8;
        }


        public void set_APP0(Header h)
        {
            //intialization
            h.APP0 = new byte[18];

            //start of APP0
            h.APP0[0] = 0xff;
            h.APP0[1] = 0xe0;

            //length
            h.APP0[2] = 0x00;
            h.APP0[3] = 0x10;

            //File Identifier Mqark
            h.APP0[4] = 0x4a;
            h.APP0[5] = 0x46;
            h.APP0[6] = 0x49;
            h.APP0[7] = 0x46;
            h.APP0[8] = 0x00;

            //Major Revision Number
            h.APP0[9] = 0x01;

            //Minor Revision Number
            h.APP0[10] = 0x01;

            //Units for x/y densities
            h.APP0[11] = 0x00;

            //xdensity
            h.APP0[12] = 0x00;
            h.APP0[13] = 0x01;

            //ydensity
            h.APP0[14] = 0x00;
            h.APP0[15] = 0x01;

            //thumbnail width and length
            h.APP0[16] = 0x00;
            h.APP0[17] = 0x00;
        }

        public void set_DQT(Header h, int[,] quant_Y, int[,] quant_CbCr)
        {
            //intialization
            h.DQT = new byte[134];

            //Marker
            h.DQT[0] = 0xff;
            h.DQT[1] = 0xdb;

            //size
            h.DQT[3] = 0x00;
            h.DQT[4] = 0x84;

            //index of Y table quant
            h.DQT[5] = 0x00;
            //Write value of quant_y
            int cpt = 0;
            int cpt2 = 0;
            for (int i = 6; i < 70; i++)
            {
                h.DQT[i] = (byte)(quant_Y[cpt, cpt2]);
                cpt2++;
                if (cpt2 == 8) { cpt++; cpt2 = 0; }
            }

            //Index of CbCr table quant
            h.DQT[70] = 0x01;
            //Write value of quant_CbCr
            cpt = 0;
            cpt2 = 0;
            for (int i = 71; i < 135; i++)
            {
                h.DQT[i] = (byte)(quant_CbCr[cpt, cpt2]);
                cpt2++;
                if (cpt2 == 8) { cpt++; cpt2 = 0; }
            }
        }

        public void set_SOF0(Header h, int heigth, int width)
        {
            //intialization
            h.SOF0 = new byte[19];

            //Marker
            h.SOF0[0] = 0xff;
            h.SOF0[1] = 0xc0;

            //length of SOF0
            h.SOF0[2] = 0x00;
            h.SOF0[3] = 0x11;

            //precision
            h.SOF0[4] = 0x08;

            //heigth and conversion in bytes
            h.SOF0[5] = (byte)(heigth & 0xff);
            h.SOF0[6] = (byte)((heigth >> 8) & 0xff);

            //width and conversion in bytes
            h.SOF0[7] = (byte)(width & 0xff);
            h.SOF0[8] = (byte)((width >> 8) & 0xff);

            //number of components
            h.SOF0[9] = 0x03;

            //Component Y
            h.SOF0[10] = 0x01;
            h.SOF0[11] = 0x11;
            h.SOF0[12] = 0x00;

            //Componenent Cb
            h.SOF0[13] = 0x02;
            h.SOF0[14] = 0x22;
            h.SOF0[15] = 0x01;

            //Component CR
            h.SOF0[16] = 0x03;
            h.SOF0[17] = 0x22;
            h.SOF0[18] = 0x01;
        }

        public void set_DHT(Header h)
        {

            //Marker
            h.DHT[0] = 0xff;
            h.DHT[1] = 0xc4;

            //need the tables to encode the rest.

        }

        public void set_SOS(Header h)
        {
            //intialization


            //Marker
            h.SOS[0] = 0xff;
            h.SOS[1] = 0xda;

            //length
            h.SOS[2] = 0x00;
            h.SOS[3] = 0x0b;

            //nomber of component
            h.SOS[4] = 0x03;

            //Component Y
            h.SOS[5] = 0x01;
            //Depends of DHT

            //Component Cb
            h.SOS[7] = 0x02;
            //Depends of DHT

            //Component Cr
            h.SOS[9] = 0x03;
            //Depends of DHT

            //ignorable bytes
            h.SOS[11] = 0x00;
            h.SOS[12] = 0x3f;
            h.SOS[13] = 0x00;
        }
        #endregion

        #region image



        #endregion

        #region End

        //End of the file
        public byte[] EOI;
        public void set_EOI()
        {
            //intialization
            EOI = new byte[2];

            //Marker
            EOI[0] = 0xff;
            EOI[1] = 0xd9;
        }

        #endregion
    }
}
