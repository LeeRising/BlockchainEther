pragma solidity ^0.4.18;

contract helloworld {
    address owner;
    string greeting;

    function Greeter(string _greeting) public {
        greeting = _greeting;
    }

    function greet() constant returns (string) {
        return greeting;
    }
}