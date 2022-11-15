#!/usr/bin/env bash

curl --location --request POST 'https://script.google.com/macros/s/AKfycbzlg9SZ3bAGSmFHfRKfFVo1sTzEtayffCeO_op1TTj6fdo3z7iI_AE9f4EjHKEaRssx/exec' \
--header 'Content-Type: application/json' \
--data-raw "{
    \"type\": \"fastlane\",
    \"link\": { \"url\": \"$1\" },
    \"application\":
    {
        \"name\": \"$3\",
        \"identifier\": \"$2\",
        \"version\": \"N/A\"
    }
}" > /dev/null
