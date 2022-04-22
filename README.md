# SCADA Application - Bottling Station - MST VGU

There are two parts in the repo:
- S7 Program: `Station4`
- WPF C# Application: `ScadaApp`

First, push `Station4` program into PLC S7-300 Bottling Station (Station 4)
- Start DI/DO address: 0
- Start AI/AO address: 256

For the WPF application, the requirements would be:
- Visual Studio 2017
- S7netplus library: https://github.com/S7NetPlus/s7netplus

I/O Mapping between PLC and C# application
- `I0.0`: level B402 reached (4B1)
- `I0.1`: tank B401 top (4B2)
- `I0.2`: tank B401 bottom (4B3)
- `I0.3`: bottle at conveyor state (4B4)
- `I0.4`: bottle at the filling point (4B5)
- `I0.5`: bottle at the end of the conveyor (4B6)

- `Q0.0`: Pump Trigger (4M1) - running with the analog output `DB100.DBD4`
- `Q0.1`: Bottling Trigger (4M2)
- `Q0.2`: Conveyor Trigger (4M3)
- `Q0.3`: Seperator Trigger (4M4)

- `DB100.DBD0`: Analog input from Ultrasonic sensor in tank B402 (Show the water level on the interface at `Tank B402`)
- `DB100.DBD4`: Analog output to control motor to pump water into Tank B402 (Control by `Slider`)


The interface:

![Alt text](application.png)

## Contact:
- Truong Nhat Hao, MST2021, VGU, haotruongnhat@gmail.com