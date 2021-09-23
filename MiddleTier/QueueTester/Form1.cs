using API.Mappers;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using QueueListeners;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueueTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var l = new PNBQueueListener(new QueueMapperConfig
            //                                (new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=pfpuat8975stg;AccountKey=X4vxwhX+z9ICDgMjT+NTqKzQvY9s6tlza2/0C13A/w/qvFn/HmzVm9WJCcmZ3H3HGoobl8oAhF3yxZXp8zyhtA==;EndpointSuffix=core.windows.net", "pnb"))
            //                            , new RestClient("https://localhost:5001")
            //                            , new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=pfpuat8975stg;AccountKey=X4vxwhX+z9ICDgMjT+NTqKzQvY9s6tlza2/0C13A/w/qvFn/HmzVm9WJCcmZ3H3HGoobl8oAhF3yxZXp8zyhtA==;EndpointSuffix=core.windows.net", "pnb"));

            //l.Run(textBox1.Text, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //var l = new SuddenDeathQueueListener(new QueueMapperConfig
            //                                (new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=pfpuat8975stg;AccountKey=X4vxwhX+z9ICDgMjT+NTqKzQvY9s6tlza2/0C13A/w/qvFn/HmzVm9WJCcmZ3H3HGoobl8oAhF3yxZXp8zyhtA==;EndpointSuffix=core.windows.net", "pnb"))
            //                            , new RestClient("https://localhost:5001")
            //                            , new BlobContainerClient("DefaultEndpointsProtocol=https;AccountName=pfpuat8975stg;AccountKey=X4vxwhX+z9ICDgMjT+NTqKzQvY9s6tlza2/0C13A/w/qvFn/HmzVm9WJCcmZ3H3HGoobl8oAhF3yxZXp8zyhtA==;EndpointSuffix=core.windows.net", "pnb"));

            //l.Run(textBox1.Text, null);
        }
    }
}
