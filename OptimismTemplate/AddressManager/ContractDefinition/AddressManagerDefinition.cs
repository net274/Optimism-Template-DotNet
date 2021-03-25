using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace OptimismTemplate.Contracts.AddressManager.ContractDefinition
{


    public partial class AddressManagerDeployment : AddressManagerDeploymentBase
    {
        public AddressManagerDeployment() : base(BYTECODE) { }
        public AddressManagerDeployment(string byteCode) : base(byteCode) { }
    }

    public class AddressManagerDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b50600080546001600160a01b03191633178082556040516001600160a01b039190911691907f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0908290a3610614806100696000396000f3fe608060405234801561001057600080fd5b50600436106100575760003560e01c8063715018a61461005c5780638da5cb5b146100665780639b2ea4bd1461008a578063bf40fac11461013b578063f2fde38b146101e1575b600080fd5b610064610207565b005b61006e6102b0565b604080516001600160a01b039092168252519081900360200190f35b610064600480360360408110156100a057600080fd5b8101906020810181356401000000008111156100bb57600080fd5b8201836020820111156100cd57600080fd5b803590602001918460018302840111640100000000831117156100ef57600080fd5b91908080601f016020809104026020016040519081016040528093929190818152602001838380828437600092019190915250929550505090356001600160a01b031691506102bf9050565b61006e6004803603602081101561015157600080fd5b81019060208101813564010000000081111561016c57600080fd5b82018360208201111561017e57600080fd5b803590602001918460018302840111640100000000831117156101a057600080fd5b91908080601f01602080910402602001604051908101604052809392919081815260200183838082843760009201919091525092955061040c945050505050565b610064600480360360208110156101f757600080fd5b50356001600160a01b031661043b565b6000546001600160a01b03163314610266576040805162461bcd60e51b815260206004820181905260248201527f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572604482015290519081900360640190fd5b600080546040516001600160a01b03909116907f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0908390a3600080546001600160a01b0319169055565b6000546001600160a01b031681565b6000546001600160a01b0316331461031e576040805162461bcd60e51b815260206004820181905260248201527f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572604482015290519081900360640190fd5b7f188466739ff00cc68bfb2367d23ae4b855264264fe1624caa8884399af23454c82826040518080602001836001600160a01b03168152602001828103825284818151815260200191508051906020019080838360005b8381101561038d578181015183820152602001610375565b50505050905090810190601f1680156103ba5780820380516001836020036101000a031916815260200191505b50935050505060405180910390a180600160006103d68561053a565b815260200190815260200160002060006101000a8154816001600160a01b0302191690836001600160a01b031602179055505050565b60006001600061041b8461053a565b81526020810191909152604001600020546001600160a01b031692915050565b6000546001600160a01b0316331461049a576040805162461bcd60e51b815260206004820181905260248201527f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572604482015290519081900360640190fd5b6001600160a01b0381166104df5760405162461bcd60e51b815260040180806020018281038252602d8152602001806105b2602d913960400191505060405180910390fd5b600080546040516001600160a01b03808516939216917f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e091a3600080546001600160a01b0319166001600160a01b0392909216919091179055565b6000816040516020018082805190602001908083835b6020831061056f5780518252601f199092019160209182019101610550565b6001836020036101000a03801982511681845116808217855250505050505090500191505060405160208183030381529060405280519060200120905091905056fe4f776e61626c653a206e6577206f776e65722063616e6e6f7420626520746865207a65726f2061646472657373a26469706673582212200bd8a213fa921f253256d7aed5f6706cfce5577ab636d8cbdcc657035377378264736f6c63430007060033";
        public AddressManagerDeploymentBase() : base(BYTECODE) { }
        public AddressManagerDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class GetAddressFunction : GetAddressFunctionBase { }

    [Function("getAddress", "address")]
    public class GetAddressFunctionBase : FunctionMessage
    {
        [Parameter("string", "_name", 1)]
        public virtual string Name { get; set; }
    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class RenounceOwnershipFunction : RenounceOwnershipFunctionBase { }

    [Function("renounceOwnership")]
    public class RenounceOwnershipFunctionBase : FunctionMessage
    {

    }

    public partial class SetAddressFunction : SetAddressFunctionBase { }

    [Function("setAddress")]
    public class SetAddressFunctionBase : FunctionMessage
    {
        [Parameter("string", "_name", 1)]
        public virtual string Name { get; set; }
        [Parameter("address", "_address", 2)]
        public virtual string Address { get; set; }
    }

    public partial class TransferOwnershipFunction : TransferOwnershipFunctionBase { }

    [Function("transferOwnership")]
    public class TransferOwnershipFunctionBase : FunctionMessage
    {
        [Parameter("address", "_newOwner", 1)]
        public virtual string NewOwner { get; set; }
    }

    public partial class AddressSetEventDTO : AddressSetEventDTOBase { }

    [Event("AddressSet")]
    public class AddressSetEventDTOBase : IEventDTO
    {
        [Parameter("string", "_name", 1, false )]
        public virtual string Name { get; set; }
        [Parameter("address", "_newAddress", 2, false )]
        public virtual string NewAddress { get; set; }
    }

    public partial class OwnershipTransferredEventDTO : OwnershipTransferredEventDTOBase { }

    [Event("OwnershipTransferred")]
    public class OwnershipTransferredEventDTOBase : IEventDTO
    {
        [Parameter("address", "previousOwner", 1, true )]
        public virtual string PreviousOwner { get; set; }
        [Parameter("address", "newOwner", 2, true )]
        public virtual string NewOwner { get; set; }
    }

    public partial class GetAddressOutputDTO : GetAddressOutputDTOBase { }

    [FunctionOutput]
    public class GetAddressOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }






}
