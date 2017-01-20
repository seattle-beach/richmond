#!/usr/bin/env bash

pushd $(dirname $0) > /dev/null
FOLDER=`pwd -P`
popd > /dev/null
INSTALLER=`basename $0`
EXTENSIONS=`ls ${FOLDER}`

for SCRIPT in $EXTENSIONS; do
  if [ "$INSTALLER" != "$SCRIPT" ]; then
    FULL_SCRIPT=${FOLDER}/${SCRIPT}
    echo "Installing ${FULL_SCRIPT} into /usr/local/bin"
    ln -sf ${FULL_SCRIPT} /usr/local/bin
  fi
done

