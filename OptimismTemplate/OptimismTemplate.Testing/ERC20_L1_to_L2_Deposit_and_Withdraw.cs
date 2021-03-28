﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ipfs;
using Ipfs.Http;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Optimism;
using Optimism.Contracts.Libraries.Resolver.Lib_AddressManager;
using Optimism.Contracts.OVM.Bridge.Tokens.OVM_L1ERC20Gateway;
using Optimism.Contracts.OVM.Bridge.Tokens.OVM_L1ERC20Gateway.ContractDefinition;
using OptimismTemplate.Contracts.ERC20;
using OptimismTemplate.Contracts.ERC20.ContractDefinition;
using OptimismTemplate.Contracts.L2DepositedERC20;
using OptimismTemplate.Contracts.L2DepositedERC20.ContractDefinition;
using OptimismTemplate.Contracts.MyERC721;
using OptimismTemplate.Contracts.MyERC721.ContractDefinition;
using Xunit;
using TransferEventDTO = OptimismTemplate.Contracts.ERC20.ContractDefinition.TransferEventDTO;

namespace OptimismTemplate.Testing
{

    public class NFTDeploymentAndMinting
    {
        public class NftMetadata
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("external_url")]
            public string ExternalUrl { get; set; }
            [JsonProperty("image")]
            public string Image { get; set; }
        }


        public async Task<IFileSystemNode> AddNftsMetadataToIpfs(NftMetadata metadata)
        {
            var ipfsClient = new IpfsClient("https://ipfs.infura.io:5001");
            using (var ms = new MemoryStream())
            {
                var serializer = new JsonSerializer();
                var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
                serializer.Serialize(jsonTextWriter, metadata);
                jsonTextWriter.Flush();
                ms.Position = 0;
                var node = await ipfsClient.FileSystem.AddAsync(ms);
                await ipfsClient.Pin.AddAsync(node.Id);
                return node;
            }
        }

        public async Task<IFileSystemNode> AddImageToIpfs(string path)
        {
            var ipfsClient = new IpfsClient("https://ipfs.infura.io:5001");
            var node = await ipfsClient.FileSystem.AddFileAsync(path);
            await ipfsClient.Pin.AddAsync(node.Id);
            return node;

        }

        [Fact]
        public async void ShouldNFT()
        {
            var web3l2 = new Web3(new Account("0x754fde3f5e60ef2c7649061e06957c29017fe21032a8017132c0078e37f6193a", 420), "http://localhost:8545");
            var ourAdddress = "0x023ffdc1530468eb8c8eebc3e38380b5bc19cc5d";
            var myERC721Deployment = new MyERC721Deployment()
            {
                BaseURI = "https://ipfs.io/ipfs/",
                Name = "OPTNETNFTS",
                Symbol = "OPTNETH",
                Gas = 7000000
            };

            var receipt = await MyERC721Service.DeployContractAndWaitForReceiptAsync(web3l2, myERC721Deployment);

            var byteCode = await web3l2.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

            var service = new MyERC721Service(web3l2, receipt.ContractAddress);
            var imageNode = await AddImageToIpfs("Images/image1.png");
            var metadataNode = await AddNftsMetadataToIpfs(new NftMetadata()
            { Name = "NethereumLovesOptimism", ExternalUrl = "https://github.com/Nethereum/OptimismTemplate/", Image = "https://ipfs.infura.io/ipfs/" + imageNode.Id.ToString() });

            var receiptMint = await service.MintRequestAndWaitForReceiptAsync(ourAdddress, metadataNode.Id.ToString());
            var mintedInfo = receiptMint
                .DecodeAllEvents<OptimismTemplate.Contracts.MyERC721.ContractDefinition.TransferEventDTO>().FirstOrDefault();

            //var tokenOfOwner = await service.TokenOfOwnerByIndexQueryAsync(ourAdddress, 0);

            var tokenMetadataUri = await service.TokenURIQueryAsync(mintedInfo.Event.TokenId);



            var client = new WebClient();

            var nftMetadataJson = await client.DownloadStringTaskAsync(new Uri(tokenMetadataUri));

            var nftMetadata = JsonConvert.DeserializeObject<NftMetadata>(nftMetadataJson);

            Assert.Equal("https://ipfs.infura.io/ipfs/" + imageNode.Id.ToString(), nftMetadata.Image);

            var ps = new ProcessStartInfo(nftMetadata.Image)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);

        }
    }


    public class ERC20_L1_to_L2_Deposit_and_Withdraw
    {

        
        //This is the addres manager for the local node
        string ADDRESS_MANAGER = "0x3e4CFaa8730092552d9425575E49bB542e329981";
        [Fact]
        public async void ShouldBeAbleToDepositErc20AndWithdrawUsingTheGateway()
        {

            //CHAINID 31337
            //PORT 9454
            var ourAdddress = "0x023ffdc1530468eb8c8eebc3e38380b5bc19cc5d";
            var web3l1 = new Web3(new Account("0x754fde3f5e60ef2c7649061e06957c29017fe21032a8017132c0078e37f6193a", 31337), "http://localhost:9545");
            var web3l2 = new Web3(new Account("0x754fde3f5e60ef2c7649061e06957c29017fe21032a8017132c0078e37f6193a", 420), "http://localhost:8545");
            var watcher = new CrossMessagingWatcherService();
            //var ourAdddress = "0xe612205919814b1995D861Bdf6C2fE2f20cDBd68";
            //var web3l1 = new Web3(new Account("", 42), "https://kovan.infura.io/v3/7238211010344719ad14a89db874158c");
            //var web3l2 = new Web3(new Account("", 69), "https://kovan.optimism.io");
            //ADDRESS_MANAGER = "0x72e6F5244828C10737cbC9659378B207246D26B2";
            var addressManagerService = new Lib_AddressManagerService(web3l1, ADDRESS_MANAGER);
            var OVM_L2CrossDomainMessenger = await addressManagerService.GetAddressQueryAsync("OVM_L2CrossDomainMessenger");
            var Proxy__OVM_L1CrossDomainMessenger = await addressManagerService.GetAddressQueryAsync("Proxy__OVM_L1CrossDomainMessenger");
 
            var tokenName = "OPNETH";
            var tokenSymbol = "OPNETH";

            var erc20TokenDeployment = new ERC20Deployment()
                { Name = tokenName, InitialSupply = 100000, Symbol = tokenSymbol, Decimals = 18 };

            //Deploy our custom token
            var tokenDeploymentReceipt = await ERC20Service.DeployContractAndWaitForReceiptAsync(web3l1, erc20TokenDeployment);

            //Deploy our ERC20 contract deployed
            var ovmL2DepositedERC20 = new L2DepositedERC20Deployment()
                { L2CrossDomainMessenger = OVM_L2CrossDomainMessenger, Name = "OVM_-" + tokenName, Symbol = "ovm_" + tokenSymbol, Decimals = 18 };

            var ovmL2DepositedERC20Receipt = await L2DepositedERC20Service.DeployContractAndWaitForReceiptAsync(web3l2, ovmL2DepositedERC20);

            var ovmL1ERC20Gateway = new OVM_L1ERC20GatewayDeployment()
                { L2DepositedERC20 = ovmL2DepositedERC20Receipt.ContractAddress, L1ERC20 = tokenDeploymentReceipt.ContractAddress, L1messenger = Proxy__OVM_L1CrossDomainMessenger };

          
            var ovmL1ERC20GatewayReceipt = await OVM_L1ERC20GatewayService.DeployContractAndWaitForReceiptAsync(web3l1, ovmL1ERC20Gateway);


            //Creating a new service
            var tokenService = new ERC20Service(web3l1, tokenDeploymentReceipt.ContractAddress);

            var gatewayService = new OVM_L1ERC20GatewayService(web3l1, ovmL1ERC20GatewayReceipt.ContractAddress);

            var l2DepositedService = new L2DepositedERC20Service(web3l2, ovmL2DepositedERC20Receipt.ContractAddress);

            //don't forget to init the l2DepositService
            await l2DepositedService.InitRequestAndWaitForReceiptAsync(ovmL1ERC20GatewayReceipt.ContractAddress);

            var balancesInL1 = await tokenService.BalanceOfQueryAsync(ourAdddress);
            var receiptApproval = await tokenService.ApproveRequestAndWaitForReceiptAsync(gatewayService.ContractHandler.ContractAddress, 1);
            var receiptDeposit = await gatewayService.DepositRequestAndWaitForReceiptAsync(new DepositFunction() { Amount = 1, Gas = 7000000 });

            balancesInL1 = await tokenService.BalanceOfQueryAsync(ourAdddress);
            //what the watcher does.. we do already have the txn receipt.. but for demo purpouses
            var messageHashes = watcher.GetMessageHashes(receiptDeposit);

            var txnReceipt = await watcher.GetCrossMessageMessageTransactionReceipt(web3l2, OVM_L2CrossDomainMessenger, messageHashes.First());

            var balancesInL2 = await l2DepositedService.BalanceOfQueryAsync(ourAdddress);

            Assert.Equal(1, balancesInL2);

            var receiptWidthdraw = await l2DepositedService.WithdrawRequestAndWaitForReceiptAsync(1);

            messageHashes = watcher.GetMessageHashes(receiptWidthdraw);

            txnReceipt = await watcher.GetCrossMessageMessageTransactionReceipt(web3l1, Proxy__OVM_L1CrossDomainMessenger, messageHashes.First());

            balancesInL2 = await l2DepositedService.BalanceOfQueryAsync(ourAdddress);

            Assert.Equal(0, balancesInL2);
        }

    }
}