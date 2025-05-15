# SauronVisionProtocol (SVP) Specification

Version: 0.1.0  
Date: May 15, 2025

## 1. Introduction

The SauronVisionProtocol (SVP) is a text-based TCP/IP protocol for communicating with the Eye of Sauron service. This protocol allows clients to send commands to the server and receive themed responses based on Lord of the Rings lore.

## 2. Transport

SVP operates over a TCP/IP connection. The default port is 9000. All messages are transmitted as UTF-8 encoded text, with each message terminated by a newline character (`\n`).

## 3. Connection Establishment

When a client connects to the server, the server sends a welcome message:

```
WELCOME SAURON_VISION_PROTOCOL
```

The client can then begin sending commands.

## 4. Command Format

Commands follow this format:

```
[COMMAND_NAME] [PARAM1] [PARAM2] ... [PARAMn]
```

Each command is terminated by a newline character (`\n`).

### 4.1 Available Commands

#### 4.1.1 PALANTIR_GAZE

Directs the Eye of Sauron's gaze to a specific location.

Format:
```
PALANTIR_GAZE [location]
```

Example:
```
PALANTIR_GAZE gondor
```

Where `[location]` is the name of a location in Middle-earth (e.g., gondor, mordor, shire).

## 5. Response Format

Responses follow this format:

```
[STATUS_CODE] [RESPONSE_TYPE] "[MESSAGE]"
```

Each response is terminated by a newline character (`\n`).

### 5.1 Status Codes

- `200`: Success
- `400`: Bad Request (client error)
- `500`: Internal Error (server error)

### 5.2 Response Types

- `VISION_GRANTED`: The Eye of Sauron successfully directed its gaze
- `VISION_DENIED`: The Eye of Sauron could not or would not direct its gaze

### 5.3 Message

A themed message describing what the Eye of Sauron saw, or an error message. The message is enclosed in double quotes.

### 5.4 Response Examples

Successful response:
```
200 VISION_GRANTED "The eye of Sauron turns to gondor. Armies of 5,000 orcs detected. The white city stands vulnerable."
```

Error response:
```
500 VISION_DENIED "Invalid command format"
```

## 6. Examples

Example session:

```
Client connects to server
Server: WELCOME SAURON_VISION_PROTOCOL
Client: PALANTIR_GAZE mordor
Server: 200 VISION_GRANTED "The eye of Sauron turns to mordor. The dark land teems with orcs and fell beasts preparing for war."
Client: PALANTIR_GAZE shire
Server: 200 VISION_GRANTED "The eye of Sauron turns to shire. Small, peaceful settlements. No threats detected. Curious halflings about."
Client: INVALID_COMMAND
Server: 500 VISION_DENIED "Invalid command format"
```

## 7. Future Extensions

Future versions of the protocol will include additional commands such as:

- `EYE_OF_SAURON [intensity] [duration]`: Controls the intensity of the gaze
- `RING_COMMAND [minion_type] [action]`: Commands minions to perform actions

These commands are not implemented in the current version.

## 8. Error Handling

If the server encounters an error while processing a command, it will respond with a `VISION_DENIED` response and an appropriate error message.

## 9. Implementation Considerations

- Clients should be prepared to handle unexpected disconnections
- The protocol is case-sensitive; commands should be sent in uppercase
- The server may close the connection after a period of inactivity
